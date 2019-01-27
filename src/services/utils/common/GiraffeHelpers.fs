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


    type Http.HttpContext with 
        member ctx.BindForm<'T> () =
                    let task = ctx.BindFormAsync<'T>()
                    task.ConfigureAwait(false).GetAwaiter().GetResult()  


    type Http.HttpContext with 
        member ctx.TryBindForm<'T> () =
                    let task = ctx.TryBindFormAsync<'T>()
                    task.ConfigureAwait(false).GetAwaiter().GetResult()                                         
                


module PipelineHelpers =

    let empty : HttpHandler =
        fun (_ : HttpFunc) (_ : HttpContext) ->     
        Task.FromResult None

 
    let removeTrailingSlash next (ctx: HttpContext) =
        let path = ctx.Request.Path.Value
        if path.EndsWith("/") && path.Length > 1 then
            redirectTo true (sprintf "%s%s" (path.Remove(path.Length-1)) ctx.Request.QueryString.Value) next ctx
        else         
            next ctx


    let (=>) user fn =
        user
        |> Option.map fn
        |> Option.defaultValue empty                   