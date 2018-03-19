namespace MyTeam

module Request =

    type PostHandler<'a,'b> = Database -> 'a -> Result<'b,Error>

    let jsonPost<'a,'b> (fn: PostHandler<'a,'b>) next (ctx: HttpContext) =
        let payload = ctx.BindJson<'a>()
        fn ctx.Database payload
        |> fromResult next ctx


    type GetHandler<'a> = Database -> Result<'a,Error>

    let jsonGet<'a,'b> (fn: GetHandler<'a>) next (ctx: HttpContext) =          
        fn ctx.Database
        |> fromResult next ctx