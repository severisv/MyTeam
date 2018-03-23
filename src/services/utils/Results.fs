namespace MyTeam
open Giraffe

type ValidationError = {
    Name: string
    Errors: string list
}

type Error = 
    | ValidationErrors of list<ValidationError>            
    | Unauthorized            
    | NotFound   

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
                    (setStatusCode 404 >=> text ("404")) next ctx    
                | _ -> failwith "Ikke implementert"
        