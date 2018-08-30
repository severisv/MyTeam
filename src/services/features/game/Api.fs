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




    let private calculateAverageAge (game: Models.Domain.Game) =
        let ages = game.Attendees
                        |> Seq.filter(fun a -> a.IsSelected)
                        |> Seq.map (fun a -> a.Member.BirthDate)
                        |> Seq.map toOption
                        |> Seq.choose id
                        |> Seq.map (fun birthDate -> 
                            (game.DateTime - birthDate).TotalDays / 365.25
                            )
                        |> Seq.toList                        
            
        Math.Round((List.sum ages / float ages.Length), 2)
    
    
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

    let getInsights club (teamName, year) (db: Database) =
        club.Teams |> Seq.tryFind (fun t -> t.ShortName.ToLower() = (toLower teamName))
        |> function
        | None -> NotFound
        | Some team ->
            let now = DateTime.Now
            db.Games.Include("Attendees.Member")
            |> Seq.filter(fun game -> 
                        game.TeamId = team.Id && 
                        game.DateTime.Year = year && 
                        game.DateTime < now &&
                        game.GameType <> Nullable(0)                        
                        )
            |> Seq.filter (fun game -> game.Attendees |> Seq.exists (fun a -> a.IsSelected))              
            |> Seq.sortBy(fun game -> game.DateTime)    
            |> Seq.map (fun g ->  
                let mf = (g.IsHomeTeam =? (g.HomeScore, g.AwayScore)) |> toOption |> Option.defaultValue 0
                let mm = (g.IsHomeTeam =? (g.AwayScore, g.HomeScore)) |> toOption |> Option.defaultValue 0
                {
                    Motstander = g.Opponent
                    Stilling = mf - mm
                    Snittalder = calculateAverageAge g
                    SpilteStåle = g.Attendees |> Seq.exists (fun p -> p.Member.FirstName = "Ståle" && p.IsSelected)
                }
            )
            |> Seq.toList
            |> fun games ->
                {
                    Snittalder = Math.Round((games |> List.sumBy (fun g -> g.Snittalder)) / float games.Length, 1)
                    Kamper = games
                }
            |> OkResult