namespace MyTeam

open Giraffe
open System
open System.Threading.Tasks
open Microsoft.AspNetCore

[<AutoOpen>]
module Framework =

    let route = routeCi
    let routef = routeCif
    let subRoute = subRouteCi

    type HttpContext = Http.HttpContext
        
    type Http.HttpContext with 
        member ctx.BindJson<'T> () =
                    let task = ctx.BindJsonAsync<'T>()
                    task.ConfigureAwait(false).GetAwaiter().GetResult()      


    type ValidationError = {
        Name: string
        Errors: string list
    }

    type Error = 
        | ValidationErrors of list<ValidationError>            
        | AuthorizationError            
        | NotFound            

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
        
                
    type Action = HttpContext -> (HttpFunc -> HttpContext -> HttpFuncResult) 


    let invoke (action: Action) (next : HttpFunc) (ctx: HttpContext) =
            (action ctx) next ctx 

    let (>->) (handler: HttpHandler) (action: Action) =
        handler >=> invoke action     

    let empty : HttpHandler =
        fun (next : HttpFunc) (_ : HttpContext) ->     
        Task.FromResult None

 