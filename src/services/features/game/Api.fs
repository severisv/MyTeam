namespace MyTeam.Games

open MyTeam.Domain
open MyTeam
open System
open Microsoft.EntityFrameworkCore

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
open MyTeam.Shared.Domain
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

    type Out = {
        GameId: Guid
        Opponent: string
        DateTime: DateTime
        IsHomeTeam: bool
        HomeScore: int option
        AwayScore: int option
        Attendees: Pl list
    }

    let private calculateAverageAge (game: Out) =
        let ages = game.Attendees
                    |> Seq.map (fun a -> a.BirthDate)
                    |> Seq.map toOption
                    |> Seq.choose id
                    |> Seq.map (fun birthDate -> 
                                    (game.DateTime - birthDate).TotalDays / 365.25                        )
                    |> Seq.toList                        
        
        Math.Round((List.sum ages / float ages.Length), 2)
    
      


    let getInsights club (teamName, year) (db: Database) =
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
            |> List.groupBy (fun (_, (id,_,_,_,_,_)) ->  id)
            |> List.map (fun (key, value) ->
                                    let (_, (_,homeScore,awayScore,opponent,dateTime,isHomeTeam)) = value |> Seq.head 
                                    {
                                        GameId = key
                                        DateTime = dateTime
                                        Opponent = opponent
                                        IsHomeTeam = isHomeTeam
                                        HomeScore = homeScore |> toOption
                                        AwayScore = awayScore |> toOption
                                        Attendees = value |> List.map (fun ((firstName, birthDate), _) ->
                                                                                                        {
                                                                                                            FirstName = firstName
                                                                                                            BirthDate = birthDate
                                                                                                        }
                                                                                )
                                    })
            |> List.sortBy (fun g -> g.DateTime)                                
            |> List.map (fun g ->  
                let mf = (g.IsHomeTeam =? (g.HomeScore, g.AwayScore)) |> Option.defaultValue 0
                let mm = (g.IsHomeTeam =? (g.AwayScore, g.HomeScore)) |> Option.defaultValue 0
                {
                    Motstander = g.Opponent
                    Stilling = mf - mm
                    Snittalder = calculateAverageAge g
                    SpilteStåle = g.Attendees |> Seq.exists (fun p -> p.FirstName = "Ståle")
                }
            )
            |> Seq.toList
            |> fun games ->
                {
                    Snittalder = Math.Round((games |> List.sumBy (fun g -> g.Snittalder)) / float games.Length, 1)
                    Kamper = games
                }
            |> OkResult