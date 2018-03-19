namespace MyTeam

open Giraffe

module Request =

    type Handler<'a,'b> = Database -> 'a -> Result<'b,Error>

    let postJson<'a,'b> (fn: Handler<'a,'b>) next (ctx: HttpContext) =          

        let payload = ctx.BindJson<'a>()

        fn ctx.Database payload
        |> fromResult next ctx


 