module MyTeam.Results

open Giraffe

let jsonResult next ctx =
    function 
    | OkResult result -> json result next ctx
    | ValidationErrors validationErrors -> (setStatusCode 400 >=> json validationErrors) next ctx
    | Unauthorized -> (setStatusCode 403 >=> json ("Ingen tilgang")) next ctx
    | NotFound -> (setStatusCode 404 >=> json ("404")) next ctx
    | Redirect url -> (redirectTo false (System.Uri.EscapeUriString url)) next ctx

let jsonPost<'a, 'b> (fn : HttpContext -> 'a -> HttpResult<'b>) next (ctx : HttpContext) =
    let payload = ctx.BindJson<'a>()
    fn ctx payload |> jsonResult next ctx

let jsonGet<'a> (fn : Database -> HttpResult<'a>) next (ctx : HttpContext) =
    fn ctx.Database |> jsonResult next ctx

let htmlResult next ctx =
    function 
    | OkResult result -> htmlView result next ctx
    | Unauthorized -> 
        let (club, user) = Tenant.get ctx
        (setStatusCode 403 >=> match club with
                               | Some club -> Views.Error.unauthorized club user
                               | None -> text "403") next ctx
    | NotFound -> 
        let (club, user) = Tenant.get ctx
        (setStatusCode 404 >=> match (club, user) with
                               | (Some club, user) -> Views.Error.notFound club user
                               | (None, _) -> text "Error") next ctx
    | Redirect url -> redirectTo false (System.Uri.EscapeUriString url) next ctx
    | ValidationErrors _ -> failwith "Ikke implementert"

let htmlGet (fn : HttpContext -> HttpResult<GiraffeViewEngine.XmlNode>) next ctx =
    fn ctx |> htmlResult next ctx

let htmlPost<'a> (fn : HttpContext -> Result<'a, string> -> HttpResult<GiraffeViewEngine.XmlNode>) 
    next (ctx : HttpContext) =
    let payload = ctx.TryBindForm<'a>()
    fn ctx payload |> htmlResult next ctx

let bind fn a =
    match a with
    | OkResult b -> fn b
    | Redirect url -> Redirect url
    | ValidationErrors ve -> ValidationErrors ve
    | NotFound -> NotFound
    | Unauthorized -> Unauthorized

let map fn a =
    match a with
    | OkResult b -> OkResult <| fn b
    | Redirect url -> Redirect url
    | ValidationErrors ve -> ValidationErrors ve
    | NotFound -> NotFound
    | Unauthorized -> Unauthorized
