namespace MyTeam.Games

open Server
open Shared.Domain
open MyTeam
open Shared
open System
open System.Linq
open Giraffe

module Api =

    let internal updateGame clubId gameId (db: Database) updateGame  =
        let (ClubId id) = clubId
        db.Games.Where(fun p -> p.ClubId = id)
          |> Seq.tryFind(fun g -> g.Id = gameId)
          |> function
           | Some game when (ClubId game.ClubId) <> clubId -> Unauthorized
           | Some game -> 
                updateGame game
                db.SaveChanges()
                |> OkResult
           | None -> NotFound      


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
    let setGamePlan clubId gameId next (ctx: HttpContext)   =
        let gamePlan = ctx.ReadBodyFromRequestAsync().ConfigureAwait(false).GetAwaiter().GetResult()       
        updateGame clubId gameId ctx.Database (fun game -> game.GamePlanState <- gamePlan)
        |> Results.jsonResult next ctx

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



    let getInsights = Insights.get