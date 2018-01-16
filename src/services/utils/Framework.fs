namespace MyTeam

open Giraffe
open Microsoft.AspNetCore

[<AutoOpen>]
module Framework =

    type HttpContext = Http.HttpContext

    type Action = HttpContext -> (HttpFunc -> HttpContext -> HttpFuncResult) * HttpContext 

    let invokeEndpoint (action: Action) (next : HttpFunc) (ctx: HttpContext) =
            let result, newCtx = (action ctx)
            result next newCtx 

    let (>->) (handler: HttpHandler) (action: Action) =
        handler >=> invokeEndpoint action 
  
