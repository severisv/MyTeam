module Server.Features.Stats.Api

open MyTeam
open MyTeam.Models.Domain
open Shared
open Shared.Domain
open Strings
open System
open Microsoft.EntityFrameworkCore
open System.Linq
open Client.Features.Players.Stats


type GameEvent =
    { EventType: GameEventType
      GameId: Guid
      PlayerId: Guid option
      AssistedById: Guid option }



let listAllPlayerStats (club: Club) (playerId: Guid) (db: Database) =

    let teamIds = club.Teams |> List.map (fun t -> t.Id)

    let treningskamp =
        Treningskamp |> GameType.toInt |> Nullable

    let events =
        let playerId = Nullable(playerId)

        db
            .GameEvents
            .Include(fun ge -> ge.Game)
            .Where(fun e ->
                (e.PlayerId = playerId || e.AssistedById = playerId)
                && e.Game.GameType <> treningskamp)
        |> Seq.toList



    let now = DateTime.Now

    let games =
        query {
            for ea in db.EventAttendances do
                where (
                    teamIds.Contains(ea.Event.TeamId.Value)
                    && ea.Event.GameType <> treningskamp
                    && ea.Event.DateTime < now
                    && ea.MemberId = playerId
                    && ea.IsSelected
                )

                select (ea.Event.TeamId.Value, ea.Event.DateTime)
        }
        |> Seq.toList

    let years =
        (games
         |> List.map (fun (teamId, dateTime) ->
             {| TeamId = teamId
                Year = dateTime.Year |})
         |> List.distinct)
        @ (teamIds
           |> List.map (fun tid -> ({| TeamId = tid; Year = 0 |})))


    let byTeamAndYear =
        (events
         |> List.groupBy (fun e ->
             {| TeamId = e.Game.TeamId.Value
                Year = e.Game.DateTime.Year |}))
        @ (events
           |> List.groupBy (fun e ->
               {| TeamId = e.Game.TeamId.Value
                  Year = 0 |}))



    let grouped =
        years
        |> List.map (fun key ->
            {| Key = key
               Items =
                byTeamAndYear
                |> List.filter (fun (k, _) -> k = key)
                |> List.collect (fun (_, values) -> values) |})

    let result =
        grouped
        |> List.groupBy (fun group -> group.Key.Year)
        |> List.map (fun (year, values) ->
            { Year = year
              Teams =
                values
                |> List.map (fun group ->

                    let gameEvents =
                        group.Items
                        |> List.map (fun ge ->
                            { AssistedById = ge.AssistedById |> Option.fromNullable
                              PlayerId = ge.PlayerId |> Option.fromNullable
                              GameId = ge.Game.Id
                              EventType = ge.Type |> int |> GameEventType.fromInt })

                    let goalCount =
                        gameEvents
                        |> List.filter (fun g -> g.PlayerId = Some playerId && g.EventType = Mål)
                        |> List.length

                    let assistCount =
                        gameEvents
                        |> List.filter (fun g -> g.AssistedById = Some playerId)
                        |> List.length

                    let yellowCards =
                        gameEvents
                        |> List.filter (fun g ->
                            g.PlayerId = Some playerId
                            && g.EventType = GameEventType.``Gult kort``)
                        |> List.length

                    let redCards =
                        gameEvents
                        |> List.filter (fun g ->
                            g.PlayerId = Some playerId
                            && g.EventType = GameEventType.``Rødt kort``)
                        |> List.length


                    { TeamId = group.Key.TeamId
                      TeamName =
                        (club.Teams
                         |> List.find (fun t -> t.Id = group.Key.TeamId))
                            .ShortName
                      Games =
                        games
                        |> List.filter (fun (teamId, datetime) ->
                            teamId = group.Key.TeamId
                            && (datetime.Year = group.Key.Year
                                || group.Key.Year = 0))
                        |> List.length
                      Goals = goalCount
                      Assists = assistCount
                      RedCards = redCards
                      YellowCards = yellowCards })
                |> List.filter (fun t -> t.Games > 0 || t.YellowCards > 0 || t.RedCards > 0)
                |> List.sortBy (fun t -> t.TeamName) }

        )
        |> List.sortByDescending (fun p -> p.Year)

    OkResult result
