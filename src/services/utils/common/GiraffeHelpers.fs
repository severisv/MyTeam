namespace MyTeam

open Giraffe
open System.Threading.Tasks
open Microsoft.AspNetCore

[<AutoOpen>]
module GiraffeHelpers =

    let route = routeCi
    let routef = routeCif
    let subRoute = subRouteCi

    type HttpContext = Http.HttpContext
        
    type Http.HttpContext with 
        member ctx.BindJson<'T> () =
                    let task = ctx.BindJsonAsync<'T>()
                    task.ConfigureAwait(false).GetAwaiter().GetResult()      
                
    type Action = HttpContext -> (HttpFunc -> HttpContext -> HttpFuncResult) 


    let invoke (action: Action) (next : HttpFunc) (ctx: HttpContext) =
            (action ctx) next ctx 

    let (>->) (handler: HttpHandler) (action: Action) =
        handler >=> invoke action     

    let empty : HttpHandler =
        fun (next : HttpFunc) (_ : HttpContext) ->     
        Task.FromResult None

 