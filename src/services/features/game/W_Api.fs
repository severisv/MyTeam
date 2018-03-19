namespace MyTeam

open MyTeam.Games
open MyTeam.Domain
open MyTeam
open Giraffe 
open System

module GameApi =

    
    [<CLIMutable>]
    type PostScore = { value: Nullable<int> }
    let setHomeScore clubId gameId db model  =
            Queries.games db clubId 
               |> Seq.tryFind(fun game -> game.Id = gameId)     
               |> function
                    | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
                    | Some game -> 
                        game.HomeScore <- model.value       
                        db.SaveChanges() 
                        |> Ok
                    | None -> Error NotFound         

    let setAwayScore clubId gameId db model  =
            Queries.games db clubId 
               |> Seq.tryFind(fun game -> game.Id = gameId)     
               |> function
                    | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
                    | Some game -> 
                        game.AwayScore <- model.value       
                        db.SaveChanges() 
                        |> Ok
                    | None -> Error NotFound                 



    let getSquad clubId gameId next (ctx: HttpContext) =
        (Queries.getSquad ctx.Database clubId gameId
         |> json) next ctx


    [<CLIMutable>]
    type GamePlanModel = { GamePlan: string }
    let setGamePlan clubId gameId db model  =
        Queries.games db clubId
          |> Seq.tryFind(fun g -> g.Id = gameId)
          |> function
           | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
           | Some game -> 
                game.GamePlan <- model.GamePlan
                db.SaveChanges()
                |> Ok
           | None -> Error NotFound                 


 
    let publishGamePlan clubId gameId db _  =
        Queries.games db clubId
          |> Seq.tryFind(fun g -> g.Id = gameId)
          |> function
           | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
           | Some game -> 
                game.GamePlanIsPublished <- Nullable true
                db.SaveChanges()
                |> Ok
           | None -> Error NotFound                 

 
    let selectPlayer = Persistence.selectPlayer        