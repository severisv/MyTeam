namespace MyTeam.Games

open MyTeam.Domain
open MyTeam
open System

module internal Helpers =
    let updateGame clubId gameId db updateGame  =
        Queries.games db clubId
          |> Seq.tryFind(fun g -> g.Id = gameId)
          |> function
           | Some game when (ClubId game.ClubId) <> clubId -> Unauthorized
           | Some game -> 
                updateGame game
                db.SaveChanges()
                |> OkResult
           | None -> NotFound      

open Helpers
open MyTeam.Domain
open MyTeam.Models.Domain
open MyTeam.Domain.Members

module Api =

    let getSquad gameId db =
        Queries.getSquad db gameId
        |> OkResult
 
    
    [<CLIMutable>]
    type PostScore = { value: Nullable<int> }
    let setHomeScore clubId gameId (ctx: HttpContext) model  =
            updateGame clubId gameId ctx.Database (fun game -> game.HomeScore <- model.value)

    let setAwayScore clubId gameId (ctx: HttpContext) model  =
           updateGame clubId gameId ctx.Database (fun game -> game.AwayScore <- model.value)


    [<CLIMutable>]
    type GamePlanModel = { GamePlan: string }
    let setGamePlan clubId gameId (ctx: HttpContext) model  =
        updateGame clubId gameId ctx.Database (fun game -> game.GamePlan <- model.GamePlan)
 
    let publishGamePlan clubId gameId (ctx: HttpContext) _  =
        updateGame clubId gameId ctx.Database (fun game -> game.GamePlanIsPublished <- Nullable true)

    let selectPlayer clubId (gameId, playerId) (ctx: HttpContext) model = 
        Persistence.selectPlayer ctx.Database clubId gameId playerId model       
    
    let publishSquad = 
        fun clubId gameId (ctx:HttpContext) _ ->
            Persistence.publishSquad ctx.Database clubId gameId
            |> Results.map (fun _ -> 
                                Notifications.clearCache ctx clubId
                            )



    type InsightsGame = {
        Motstander: string
        Stilling: int
        Snittalder: float
        SpilteStåle: bool
    }

    type Insights = {
        Snittalder: float
        Kamper: InsightsGame list
    }

    type Pl = {
        FirstName: string
        BirthDate: Nullable<DateTime>
    }

    let private calculateAverageAge dateTime attendees =
        let ages = attendees 
                    |> Seq.map ((fun a -> a.BirthDate) >> toOption)
                    |> Seq.choose id
                    |> Seq.map (fun birthDate -> 
                                    (dateTime - birthDate).TotalDays / 365.25)
                    |> Seq.toList                        
        
        Math.Round((List.sum ages / float ages.Length), 2)
    
      


    let getInsights (club: Domain.Club) (teamName, year) (db: Database) =
        club.Teams |> Seq.tryFind (fun t -> t.ShortName.ToLower() = (toLower teamName))
        |> function
        | None -> NotFound
        | Some team ->
            let now = DateTime.Now
            query {
                for game in db.Games do
                where (
                     game.TeamId = team.Id && 
                     game.DateTime.Year = year && 
                     game.DateTime < now &&
                     game.GameType <> Nullable(0) 
                )
                leftOuterJoin ge in db.EventAttendances on (game.Id = ge.EventId) into result
                for ge in result do
                where (ge.IsSelected) 
                select (
                        (ge.Member.FirstName, ge.Member.BirthDate),
                            (game.Id,
                             game.HomeScore,
                             game.AwayScore,
                             game.Opponent,
                             game.DateTime,
                             game.IsHomeTeam))
            }               
            |> Seq.toList
            |> List.sortBy(fun  (_, (_,_,_,_,dateTime,_)) -> dateTime)    
            |> List.groupBy (fun (_, (id,_,_,_,_,_)) ->  id)
            |> List.map (fun (_, value) ->
                                    let (_, (_,homeScore,awayScore,opponent,dateTime,isHomeTeam)) = value |> Seq.head                                
                                    let attendees = 
                                        value |> List.map (fun ((firstName, birthDate), _) ->
                                                                                            {
                                                                                                FirstName = firstName
                                                                                                BirthDate = birthDate
                                                                                            }
                                                                                )
                                    let mf = (isHomeTeam =? (homeScore, awayScore)) |> toOption |> Option.defaultValue 0
                                    let mm = (isHomeTeam =? (awayScore, homeScore)) |> toOption |> Option.defaultValue 0
                                    {
                                        Motstander = opponent
                                        Stilling = mf - mm
                                        Snittalder = calculateAverageAge dateTime attendees
                                        SpilteStåle = attendees |> Seq.exists (fun p -> p.FirstName = "Ståle")
                                    }
            )        
            |> Seq.toList
            |> fun games ->
                {
                    Snittalder = Math.Round((games |> List.sumBy (fun g -> g.Snittalder)) / float games.Length, 1)
                    Kamper = games
                }
            |> OkResult