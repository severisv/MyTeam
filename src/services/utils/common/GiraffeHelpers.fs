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

    let empty : HttpHandler =
        fun (_ : HttpFunc) (_ : HttpContext) ->     
        Task.FromResult None

 