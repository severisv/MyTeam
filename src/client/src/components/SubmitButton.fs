module Client.Components.SubmitButton

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Components
open Shared
open Fable.Core.JsInterop
open Thoth.Json
open Shared.Components.Base

let sendRecord2<'T> method (url : string) (record : 'T) (properties : RequestProperties list) =
    let defaultProps =
        [ RequestProperties.Method method
       ; requestHeaders [ ContentType "application/json"; Custom ("json-mode", "fable") ]
       ; RequestProperties.Body !^(Encode.Auto.toString (0, record)) ]

    let init = List.append defaultProps properties
    GlobalFetch.fetch (RequestInfo.Url url, requestProps init)

type Endpoint<'a> =
    | Put of string * 'a option
    | Post of string * 'a option
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
    { Size : ButtonSize
      IsSubmitted : bool
      Text : React.ReactElement
      ButtonStyle : ButtonType
      SubmittedText : string
      Endpoint : Endpoint<'a>
      IsDisabled : bool
      OnError : (SubmitError -> unit) option
      OnSubmit : (string -> unit) option }

   
let defaultButton size buttonStyle attr content = btn ([ buttonStyle; size ] @ attr) content

type SubmitButton<'a>(props) =
    inherit Component<SubmitButtonProps<'a>, SubmitButtonState>(props)

    do
        base.setInitState (if props.IsSubmitted then Submitted
                           else Default)

    member this.handleClick _ =
        let props = this.props
        this.setState (fun _ _ -> Posting)
        promise {
            let! res = match props.Endpoint with
                       | Post(url, payload) -> sendRecord2 HttpMethod.POST url payload []
                       | Put(url, payload) -> sendRecord2 HttpMethod.PUT url payload []
                       | Delete url -> fetch url [Method HttpMethod.DELETE ]

            match (res.Status, props.OnError, res.Ok) with
            | (400, Some onError, _) ->
                  let! validationErrors = res.json<ValidationError array>()
                  this.setState (fun state props -> Default)
                  let result = validationErrors |> Array.toList
                  props.OnError.Value(ValidationError result)

            | (_, _, false) ->
                printf "%i" res.Status
                failwithf "Received %O from server: %O" res.Status res.StatusText

            | (_, _, true) ->
                match props.OnSubmit with
                | Some onSubmit ->
                    this.setState (fun state props -> Default)
                    let! result = res.text()
                    onSubmit(result)
                | None -> this.setState (fun state props -> Submitted)
        }
        |> Promise.catch (fun e ->
               Browser.console.error <| sprintf "%O" e
               match props.OnError with
               | Some onError ->
                   this.setState (fun _ _ -> Error)
                   onError (Exception e.Message)
               | None -> this.setState (fun _ _ -> Error))
        |> Promise.start

    override this.render() =                
        let props = this.props
        let state = this.state
        let handleClick = this.handleClick
        let defaultButton = defaultButton props.Size props.ButtonStyle
        match state with
        | Submitted ->
            btn [ Success
                  props.Size
                  Class "disabled" ] [Icons.checkCircle
                                      whitespace
                                      str props.SubmittedText]
        | Posting -> defaultButton [ Class "disabled" ] [ Icons.spinner ]
        | Error ->
            fragment [] [ defaultButton [ OnClick handleClick ] [ props.Text ]
                          Labels.error ]
        | Default when props.IsDisabled -> defaultButton [ Class "disabled" ] [ props.Text ]
        | Default -> defaultButton [ OnClick handleClick ] [ props.Text ]

let render getProps = ofType<SubmitButton<'a>, _, _> (getProps { Size = Lg
                                                                 IsSubmitted = false
                                                                 Text = str ""
                                                                 ButtonStyle = Primary
                                                                 SubmittedText = ""
                                                                 Endpoint = Post("", None)
                                                                 IsDisabled = false
                                                                 OnError = None
                                                                 OnSubmit = None }) []
