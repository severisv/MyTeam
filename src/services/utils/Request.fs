namespace MyTeam

module Request =

    type Handler<'a,'b> = Database -> 'a -> Result<'b,Error>

    let jsonPost<'a,'b> (fn: Handler<'a,'b>) next (ctx: HttpContext) =          

        let payload = ctx.BindJson<'a>()

        fn ctx.Database payload
        |> fromResult next ctx


 