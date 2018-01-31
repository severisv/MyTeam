namespace MyTeam

open MyTeam.Games
open MyTeam.Domain
open MyTeam
open Giraffe 

module GameApi =

    
    [<CLIMutable>]
    type PostScore = {
        value: int
    }

    let setHomeScore clubId gameId next (ctx: HttpContext) =
            let input = ctx.BindJson<PostScore>()
            { Home = Some input.value; Away = None } 
            |> Persistence.setScore ctx.Database clubId gameId       
            |> fromResult next ctx

    let setAwayScore clubId gameId next (ctx: HttpContext) =
        let input = ctx.BindJson<PostScore>()
        { Home = None; Away = Some input.value } 
        |> Persistence.setScore ctx.Database clubId gameId       
        |> fromResult next ctx        
