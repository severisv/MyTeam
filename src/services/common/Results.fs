module Server.Results

open Giraffe
open MyTeam
open Shared
open Common

let json result next (ctx: HttpContext) =
     match ctx.TryGetRequestHeader "json-mode" with
            | Some "fable" ->
                    (setHttpHeader "Content-Type" "application/json" >=>
                        setBodyFromString (Json.fableSerialize result)) next ctx
            | _ -> json result next ctx

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
        (setStatusCode 403 >=> (Tenant.get ctx
                               |> function
                               | (Some club, user) -> Views.Error.unauthorized club user
                               | (None, _) -> text "403")) next ctx
    | NotFound -> 
        (setStatusCode 404 >=> (Tenant.get ctx
                               |> function
                               | (Some club, user) -> Views.Error.notFound club user
                               | (None, _) -> text "Error")) next ctx
    | Redirect url -> redirectTo false (System.Uri.EscapeUriString url) next ctx
    | ValidationErrors _ -> failwith "Ikke implementert"

let htmlGet (fn : HttpContext -> HttpResult<GiraffeViewEngine.XmlNode>) next ctx =
    fn ctx |> htmlResult next ctx

let htmlPost<'a> (fn : Result<'a, string> -> HttpContext -> HttpResult<GiraffeViewEngine.XmlNode>) 
    next (ctx : HttpContext) =
    let payload = ctx.TryBindForm<'a>()
    fn payload ctx |> htmlResult next ctx

let bind fn =
    function
    | OkResult b -> fn b
    | Redirect url -> Redirect url
    | ValidationErrors ve -> ValidationErrors ve
    | NotFound -> NotFound
    | Unauthorized -> Unauthorized

let map fn =
    function
    | OkResult b -> OkResult <| fn b
    | Redirect url -> Redirect url
    | ValidationErrors ve -> ValidationErrors ve
    | NotFound -> NotFound
    | Unauthorized -> Unauthorized
