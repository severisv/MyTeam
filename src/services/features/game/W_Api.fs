namespace MyTeam

open MyTeam.Games
open MyTeam.Domain
open MyTeam
open Giraffe 
open System

module GameApi =

    
    [<CLIMutable>]
    type PostScore = {
        value: Nullable<int>
    }

    let setHomeScore clubId gameId (db: Database) model  =
            Queries.games db clubId 
               |> Seq.tryFind(fun game -> game.Id = gameId)     
               |> function
                    | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
                    | Some game -> 
                        game.HomeScore <- model.value       
                        db.SaveChanges() |> ignore
                        Ok (Object())
                    | None -> Error NotFound         

    let setAwayScore clubId gameId (db: Database) model  =
            Queries.games db clubId 
               |> Seq.tryFind(fun game -> game.Id = gameId)     
               |> function
                    | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
                    | Some game -> 
                        game.AwayScore <- model.value       
                        db.SaveChanges() |> ignore
                        Ok (Object())
                    | None -> Error NotFound                 



    let getSquad clubId gameId next (ctx: HttpContext) =
        (Queries.getSquad ctx.Database clubId gameId
         |> json) next ctx


    [<CLIMutable>]
    type GamePlanModel = { GamePlan: string }
    let setGamePlan clubId gameId next (ctx: HttpContext) =
        let model = ctx.BindJson<GamePlanModel>()
        let game = Queries.games ctx.Database clubId
                  |> Seq.find(fun g -> g.Id = gameId)
        game.GamePlan <- model.GamePlan
        ctx.Database.SaveChanges() |> ignore
        next ctx

    let publishGamePlan clubId gameId next (ctx: HttpContext) =
        let db = ctx.Database
        let game = Queries.games ctx.Database clubId
                  |> Seq.find(fun g -> g.Id = gameId)

        game.GamePlanIsPublished <- Nullable true
        db.SaveChanges() |> ignore
        next ctx


    let selectPlayer = Persistence.selectPlayer        