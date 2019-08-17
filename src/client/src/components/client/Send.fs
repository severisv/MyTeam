module Client.Components.Send

open Browser
open Fable.React
open Fable.React.Props
open Fetch
open Shared.Components
open Shared
open Shared.Components.Base
open Client.Util
open Shared.Util
open Shared.Util.ReactHelpers

type Endpoint<'a> =
    | Put of string * (unit -> string) option
    | Post of string * (unit -> string) option
    | Delete of string

type SendState =
    | Default
    | Submitted
    | Sending
    | Error

type SendError =
    | Exception of string
    | ValidationError of ValidationError list
        
type ReactComponent = IHTMLProp list -> ReactElement list -> ReactElement

type SendProps<'a> =
    { IsSent : bool option
      SentClass: string option
      Spinner: ReactElement list option
      SentIndicator: ReactElement list option
      SendElement : ReactComponent * IHTMLProp list * ReactElement list
      SentElement : ReactComponent * IHTMLProp list * ReactElement list
      Endpoint : Endpoint<'a>
      IsDisabled : bool
      OnError : (SendError -> unit) option
      OnSubmit : (string -> unit) option }
   

let defaultButton size buttonStyle attr content = btn ([ buttonStyle; size ] @ attr) content


let internal handleClick props setState _ =
    setState (fun _ _ -> Sending)
    promise {
        let! res = match props.Endpoint with
                   | Post(url, payload) -> Http.send HttpMethod.POST url (payload |> Option.defaultValue (fun () -> "")) []
                   | Put(url, payload) -> Http.send HttpMethod.PUT url (payload |> Option.defaultValue (fun () -> "")) []
                   | Delete url -> fetch url [Method HttpMethod.DELETE ]

        match (res.Status, props.OnError, res.Ok) with
        | (400, Some onError, _) ->
              let! validationErrors = res.json<ValidationError array>()
              setState (fun state props -> Default)
              let result = validationErrors |> Array.toList
              props.OnError.Value(ValidationError result)

        | (_, _, false) ->
            printf "%i" res.Status
            failwithf "Received %O from server: %O" res.Status res.StatusText

        | (_, _, true) ->
            match props.OnSubmit with
            | Some onSubmit ->
                let! result = res.text()
                onSubmit result
                setState (fun state props -> Default)
            | None -> setState (fun state props -> Submitted)
    }
    |> Promise.catch (fun e ->
           Dom.console.error (sprintf "%O" e)
           match props.OnError with
           | Some onError ->
               setState (fun _ _ -> Error)
               onError (Exception e.Message)
           | None -> setState (fun _ _ -> Error))
    |> Promise.start




let sendElement getProps =
    let props =  getProps { IsSent = None
                            SentIndicator = Some [Icons.checkCircle; whitespace]
                            SentClass = Some "disabled"
                            Spinner = Some [Icons.spinner]
                            Endpoint = Post("", None)
                            IsDisabled = false
                            OnError = None
                            OnSubmit = None
                            SendElement = btn, [Primary;Lg], []
                            SentElement = btn, [Success;Lg], []}
    
    let sendElement, sendAttr, sendChildren = props.SendElement
    let sentElement, sentAttr, sentChildren = props.SentElement
    
    komponent<SendProps<'a>, SendState>
        props
        Default
        None
        (fun (props, state, setState) ->
            
            let handleClick = handleClick props setState
            
            let state =
                match (state, props.IsSent) with
                | (Default, Some true) -> Submitted
                | (Submitted, Some false) -> Submitted
                | _ -> state
                    
            match state with
            | Submitted ->
                    sentElement (Html.mergeClasses [Class (props.SentClass |> Option.defaultValue "")] sentAttr)
                                ((props.SentIndicator |> Option.defaultValue [empty]) @ sentChildren)
            | Sending -> sendElement ([Class "disabled" ] @ sendAttr) (props.Spinner |> Option.defaultValue sendChildren)
            | Error ->
                fragment [] [ sendElement (Html.mergeClasses [OnClick handleClick] sendAttr) sendChildren
                              Labels.error ]
            | Default when props.IsDisabled -> sendElement (Html.mergeClasses [Class "disabled" ] sendAttr) sendChildren
            | Default -> sendElement (Html.mergeClasses [OnClick handleClick ] sendAttr) sendChildren)
        
