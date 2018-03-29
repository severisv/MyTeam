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


    let jsonPost<'a,'b> (fn: Database -> 'a -> Result<'b,Error>) next (ctx: HttpContext) =
        let payload = ctx.BindJson<'a>()
        fn ctx.Database payload
        |> jsonResult next ctx

    let jsonGet<'a> (fn: Database -> Result<'a,Error>) next (ctx: HttpContext) =          
        fn ctx.Database
        |> jsonResult next ctx


    let htmlGet (fn: (HttpContext -> Result<GiraffeViewEngine.XmlNode, Error>)) next ctx =    
            fn ctx
            |> function       
            | Ok result -> htmlView result next ctx       
            | Error e -> 
                (match e with
                | Unauthorized ->
                    setStatusCode 403 >=> text "Ingen tilgang"    
                | NotFound ->
                    setStatusCode 404 >=>
                     (Tenant.get ctx 
                     |> function
                        | (Some club, user) -> 
                            Views.Error.notFound club user
                        | (None, _) -> text "Error"
                     )
                | _ -> failwith "Ikke implementert") next ctx
                
        