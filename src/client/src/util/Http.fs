module Client.Util.Http


open Fable.Import
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json

type GetParameters<'a> = {
    OnSuccess: 'a -> unit
    OnError: string -> unit
}

let get url
        deserialize
        actions =    
    promise { 
        let! res = fetch url [requestHeaders [Custom ("json-mode", "fable")] ]
        if not res.Ok then failwithf "Received %O from server: %O" res.Status res.StatusText
        let! text = res.text()
        let json = deserialize text       
        json
        |> function
        | Ok res -> actions.OnSuccess res
        | Error e ->
            Browser.console.error e
            actions.OnError e
    }
    |> Promise.catch(fun e -> 
           Browser.console.error <| sprintf "%O" e
           actions.OnError e.Message
    )
    |> Promise.start
