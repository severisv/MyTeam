module MyTeam.Games.Insights

open MyTeam
open Shared
open System

type InsightsGame =
    { Motstander : string
      Stilling : int
      Snittalder : float }

type Insights =
    { Snittalder : float
      Kamper : InsightsGame list }

type Player =
    { FirstName : string
      BirthDate : Nullable<DateTime> }

let private calculateAverageAge dateTime attendees =
    let ages =
        attendees
        |> Seq.map((fun a -> a.BirthDate) >> toOption)
        |> Seq.choose id
        |> Seq.map(fun birthDate -> (dateTime - birthDate).TotalDays / 365.25)
        |> Seq.toList
    Math.Round((List.sum ages / float ages.Length), 2)

let get (club : Domain.Club) (teamName, year) (db : Database) =
    club.Teams
    |> List.tryFind(fun t -> t.ShortName.ToLower() = (toLower teamName))
    |> function 
    | None -> NotFound
    | Some team -> 
        let now = DateTime.Now
        query { 
            for game in db.Games do
                where
                    (game.TeamId.Value = team.Id && game.DateTime.Year = year && game.DateTime < now 
                     && game.GameType <> Nullable(0))
                leftOuterJoin ge in db.EventAttendances on (game.Id = ge.EventId) into result
                for ge in result do
                    where(ge.IsSelected)
                    select
                        ((ge.Member.FirstName, ge.Member.BirthDate), 
                         (game.Id, game.HomeScore, game.AwayScore, game.Opponent, game.DateTime, 
                          game.IsHomeTeam))
        }
        |> Seq.toList
        |> List.sortBy(fun (_, (_, _, _, _, dateTime, _)) -> dateTime)
        |> List.groupBy(fun (_, (id, _, _, _, _, _)) -> id)
        |> List.map(fun (_, value) -> 
               let (_, (_, homeScore, awayScore, opponent, dateTime, isHomeTeam)) =
                   value |> Seq.head
               
               let attendees =
                   value
                   |> List.map(fun ((firstName, birthDate), _) -> 
                          { FirstName = firstName
                            BirthDate = birthDate })
               
               let mf =
                   isHomeTeam =? (homeScore, awayScore)
                   |> toOption
                   |> Option.defaultValue 0
               
               let mm =
                   isHomeTeam =? (awayScore, homeScore)
                   |> toOption
                   |> Option.defaultValue 0
               
               { Motstander = opponent
                 Stilling = mf - mm
                 Snittalder = calculateAverageAge dateTime attendees })
        |> Seq.toList
        |> fun games -> 
            { Snittalder =
                  Math.Round((games |> List.sumBy(fun g -> g.Snittalder)) / float games.Length, 1)
              Kamper = games }
        |> OkResult
