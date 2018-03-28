namespace MyTeam
open Giraffe

module Results =
    let jsonResult next ctx =
        function
        | Ok result -> json result next ctx       
        | Error e -> 
            match e with
            | ValidationErrors validationErrors ->
                (setStatusCode 400 >=> json validationErrors) next ctx    
            | Unauthorized ->
                (setStatusCode 403 >=> json ("Ingen tilgang")) next ctx    
            | NotFound ->
                (setStatusCode 404 >=> json ("404")) next ctx    




    type PostHandler<'a,'b> = Database -> 'a -> Result<'b,Error>

    let jsonPost<'a,'b> (fn: PostHandler<'a,'b>) next (ctx: HttpContext) =
        let payload = ctx.BindJson<'a>()
        fn ctx.Database payload
        |> jsonResult next ctx


    type GetHandler<'a> = Database -> Result<'a,Error>

    let jsonGet<'a> (fn: GetHandler<'a>) next (ctx: HttpContext) =          
        fn ctx.Database
        |> jsonResult next ctx


    let htmlGet (fn: (HttpContext -> Result<GiraffeViewEngine.XmlNode, Error>)) next ctx =    
            fn ctx
            |> function       
            | Ok result -> htmlView result next ctx       
            | Error e -> 
                match e with
                | Unauthorized ->
                    (setStatusCode 403 >=> text ("Ingen tilgang")) next ctx    
                | NotFound ->
                    (setStatusCode 404 >=>
                     (Tenant.get ctx 
                     |> function
                        | (Some club, user) -> 
                            Views.Error.notFound club user
                        | (None, _) -> text "Error"
                     )) next ctx
                | _ -> failwith "Ikke implementert"
        