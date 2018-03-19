namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.AspNetCore
open System
open System.Threading.Tasks

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
        fun (next : HttpFunc) (ctx : HttpContext) ->     
        Task.FromResult None

    let (=??) (first: string) (second: string) =
        if not <| String.IsNullOrWhiteSpace(first) then first else second          


    let (=?) (condition: bool) (first, second) =
        if condition then first else second       

    let (|??) option alternative =
        match option with
        | Some value -> value
        | None -> alternative       


    let (>>=) result fn =
        result |> Result.bind(fn)    

    let mergeAttributes (a: XmlAttribute list) (b: XmlAttribute list) =
        a @ b |> List.groupBy (function
                                | KeyValue (key, _) -> key
                                | Boolean key -> key)
              |> List.map (fun (key, values) ->
                            let values = values |> List.map(function
                                                    | KeyValue (_, value) -> value
                                                    | Boolean key -> key)
                                                |> String.concat " "
                            KeyValue (key, values)                                            
                          )                  