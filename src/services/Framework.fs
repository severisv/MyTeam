namespace MyTeam

open Giraffe
open Microsoft.AspNetCore
open Microsoft.Extensions.DependencyInjection;
open System
open System.Net.Mail

[<AutoOpen>]
module Framework =

    let route = routeCi

    let routef = routeCif

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


    type ValidationError = {
        Name: string
        Errors: string list
    }

    type Error = 
        | ValidationErrors of list<ValidationError>            

    let fromResult next ctx (result: Result<'T, Error>) =
        match result with
            | Ok r -> 
                match r with
                    | _ -> json r next ctx
            | Error e -> 
                let (ValidationErrors validationErrors) = e
                (setStatusCode 400 >=> json validationErrors) next ctx    

    type Action = HttpContext -> (HttpFunc -> HttpContext -> HttpFuncResult) 

    let invoke (action: Action) (next : HttpFunc) (ctx: HttpContext) =
            (action ctx) next ctx 

    let (>->) (handler: HttpHandler) (action: Action) =
        handler >=> invoke action     


    let (=??) (first: string) (second: string) =
        if not <| String.IsNullOrWhiteSpace(first) then first else second          


    let (=?) (condition: bool) (first, second) =
        if condition then first else second       

    let (|??) option alternative =
        match option with
        | Some value -> value
        | None -> alternative       