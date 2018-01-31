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

