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

