namespace MyTeam

open Giraffe

module Request =
                      
    let postJson<'a> fn next (ctx: HttpContext) =          

        let payload = ctx.BindJson<'a>()

        fn ctx.Database payload
        |> fromResult next ctx


 