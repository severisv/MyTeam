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

    let setHomeScore clubId gameId next (ctx: HttpContext) =
            let db = ctx.Database
            let game = Queries.games db clubId 
                       |> Seq.find(fun game -> game.Id = gameId)                    

            game.HomeScore <- ctx.BindJson<PostScore>().value
            db.SaveChanges() |> ignore
            next ctx


    let setAwayScore clubId gameId next (ctx: HttpContext) =
            let db = ctx.Database
            let game = Queries.games db clubId 
                       |> Seq.find(fun game -> game.Id = gameId)                    

            game.AwayScore <- ctx.BindJson<PostScore>().value
            db.SaveChanges() |> ignore
            next ctx



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