module Client.Components.SubmitButton

open Browser
open Fable.React
open Fable.React.Props
open Fetch
open Shared.Components
open Shared
open Shared.Components.Base
open Client.Util
open Shared.Util.ReactHelpers

type Endpoint<'a> =
    | Put of string * (unit -> string) option
    | Post of string * (unit -> string) option
    | Delete of string

type SubmitButtonState =
    | Default
    | Submitted
    | Posting
    | Error

type SubmitError =
    | Exception of string
    | ValidationError of ValidationError list

type SubmitButtonProps<'a> =
    { IsSubmitted : bool
      Text : ReactElement
      ButtonStyle : ButtonType
      Size : ButtonSize
      SubmittedText : string
      Endpoint : Endpoint<'a>
      IsDisabled : bool
      OnError : (SubmitError -> unit) option
      OnSubmit : (string -> unit) option }
   

let defaultButton size buttonStyle attr content = btn ([ buttonStyle; size ] @ attr) content


let internal handleClick props setState _ =
    setState (fun _ _ -> Posting)
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




let render2 props
            (sendElement: IHTMLProp list -> ReactElement seq -> ReactElement)
            (submittedElement: IHTMLProp list -> ReactElement seq -> ReactElement)  =   
    
    komponent<SubmitButtonProps<'a>, SubmitButtonState>
        props
        (if props.IsSubmitted then Submitted else Default)
        None
        (fun (props, state, setState) ->
            
            let handleClick = handleClick props setState
            match state with
            | Submitted ->
                    submittedElement [Class "disabled"]
                                     [Icons.checkCircle
                                      whitespace
                                      str props.SubmittedText]
            | Posting -> sendElement [Class "disabled" ] [Icons.spinner]
            | Error ->
                fragment [] [ sendElement [ OnClick handleClick ] [props.Text]
                              Labels.error ]
            | Default when props.IsDisabled -> sendElement [Class "disabled" ] [props.Text]
            | Default -> sendElement [OnClick handleClick ] [props.Text])
        
let render getProps =
     let props = (getProps { Size = Lg
                             IsSubmitted = false
                             Text = str ""
                             SubmittedText = ""
                             ButtonStyle = Primary
                             Endpoint = Post("", None)
                             IsDisabled = false
                             OnError = None
                             OnSubmit = None })
     let defaultButton = defaultButton props.Size props.ButtonStyle

     render2 props defaultButton (fun attr -> btn ([Success;props.Size] @ attr))
        