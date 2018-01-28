namespace MyTeam

open Giraffe
open Microsoft.AspNetCore
open Microsoft.Extensions.DependencyInjection;

[<AutoOpen>]
module Framework =

    type HttpContext = Http.HttpContext
    let getService<'T> (ctx: HttpContext) =             
        ctx.RequestServices.GetService<'T>()

        
    type Http.HttpContext with 
        member  ctx.BindJson<'T> () =
                        let task = task {
                                    let! result = ctx.BindJsonAsync<'T>()
                                    return result
                                }
                        task.ConfigureAwait(false).GetAwaiter().GetResult()              


    type Error = 
        | ValidationError of string[]                    


    type Action = HttpContext -> (HttpFunc -> HttpContext -> HttpFuncResult) 

    let invokeEndpoint (action: Action) (next : HttpFunc) (ctx: HttpContext) =
            (action ctx) next ctx 

    let (>->) (handler: HttpHandler) (action: Action) =
        handler >=> invokeEndpoint action     