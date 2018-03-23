namespace MyTeam
open Giraffe

type ValidationError = {
    Name: string
    Errors: string list
}

type Error = 
    | ValidationErrors of list<ValidationError>            
    | AuthorizationError            
    | NotFound   

module Request =
    let fromResult next ctx (result: Result<'T, Error>) =
        match result with
            | Ok r -> 
                match r with
                | _ -> json r next ctx
            | Error e -> 
                match e with
                | ValidationErrors validationErrors ->
                    (setStatusCode 400 >=> json validationErrors) next ctx    
                | AuthorizationError ->
                    (setStatusCode 403 >=> json ("Ingen tilgang")) next ctx    
                | NotFound ->
                    (setStatusCode 404 >=> json ("404")) next ctx    



    type PostHandler<'a,'b> = Database -> 'a -> Result<'b,Error>

    let jsonPost<'a,'b> (fn: PostHandler<'a,'b>) next (ctx: HttpContext) =
        let payload = ctx.BindJson<'a>()
        fn ctx.Database payload
        |> fromResult next ctx



    type GetHandler<'a> = Database -> Result<'a,Error>

    let jsonGet<'a,'b> (fn: GetHandler<'a>) next (ctx: HttpContext) =          
        fn ctx.Database
        |> fromResult next ctx


    let htmlGet<'a,'b> (fn: GetHandler<'a>) next (ctx: HttpContext) =          
        fn ctx.Database
        |> fromResult next ctx