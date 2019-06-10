module Client.Util.Http


open Fable.Import
open Thoth.Json
open Fetch
open Fable.Core.JsInterop

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
            Browser.Dom.console.error(sprintf "%O" e)
            actions.OnError e
    }
    |> Promise.catch(fun e -> 
           Browser.Dom.console.error(sprintf "%O" e)
           actions.OnError e.Message
    )
    |> Promise.start


let inline sendRecord<'T> method (url : string) (record : 'T) (properties : RequestProperties list) =
    let defaultProps =
        [ RequestProperties.Method method
       ; requestHeaders [ ContentType "application/json"; Custom("json-mode", "fable") ]
       ; RequestProperties.Body !^(Encode.Auto.toString (0, record)) ]

    let init = List.append defaultProps properties
    GlobalFetch.fetch (RequestInfo.Url url, requestProps init)
    
let inline send<'T> method (url : string) (getPayload: unit -> string) (properties : RequestProperties list) =
    let defaultProps =
        [ RequestProperties.Method method
       ; requestHeaders [ ContentType "application/json"; Custom("json-mode", "fable") ]
       ; RequestProperties.Body !^(getPayload()) ]

    let init = List.append defaultProps properties
    GlobalFetch.fetch (RequestInfo.Url url, requestProps init)