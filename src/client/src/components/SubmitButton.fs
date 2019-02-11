module MyTeam.Client.Components.SubmitButton

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Components
open MyTeam
open Fable.Core.JsInterop
open Thoth.Json
open Shared.Components.Base



type LowercaseValidationError = {
    name: string
    errors: string list
}

let toUpperCaseValidationError er = { Name = er.name; Errors = er.errors  }

let postRecord2<'T> (url : string) (record : 'T) (properties : RequestProperties list) =
    let defaultProps =
        [ RequestProperties.Method HttpMethod.POST
       ; requestHeaders [ ContentType "application/json" ]
       ; RequestProperties.Body !^(Encode.Auto.toString (0, record)) ]

    let init = List.append defaultProps properties
    GlobalFetch.fetch (RequestInfo.Url url, requestProps init)


type Endpoint<'a> =
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
      Text : string
      SubmittedText : string
      Endpoint : Endpoint<'a>
      IsDisabled : bool
      OnError : (SubmitError -> unit) option
      OnSubmit : (unit -> unit) option }

let defaultProps =
    { Size = Lg
      IsSubmitted = false
      Text = ""
      SubmittedText = ""
      Endpoint = Post("", None)
      IsDisabled = false
      OnError = None
      OnSubmit = None }

let defaultButton size attr content = btn ([ Primary; size ] @ attr) content

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
                       | Post(url, payload) -> postRecord2 url payload []
                       | Delete url -> fetch url [ Method HttpMethod.DELETE ]

            match (res.Status, props.OnError, res.Ok) with
            | (400, Some onError, _) ->
                  let! validationErrors = res.json<LowercaseValidationError array>()
                  this.setState (fun state props -> Default)
                  let result = validationErrors
                               |> Array.map toUpperCaseValidationError
                               |> Array.toList
                  props.OnError.Value(ValidationError result)

            | (_, _, false) ->
                printf "%i" res.Status
                failwithf "Received %O from server: %O" res.Status res.StatusText

            | (_, _, true) ->
                match props.OnSubmit with
                | Some onSubmit ->
                    this.setState (fun state props -> Default)
                    onSubmit()
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
        let defaultButton = defaultButton props.Size
        match state with
        | Submitted ->
            btn [ Success
                  props.Size
                  Class "disabled" ] [ Icons.checkCircle
                                       whitespace
                                       str props.SubmittedText ]
        | Posting -> defaultButton [ Class "disabled" ] [ Icons.spinner ]
        | Error ->
            fragment [] [ defaultButton [ OnClick handleClick ] [ str props.Text ]
                          Labels.error ]
        | Default when props.IsDisabled -> defaultButton [ Class "disabled" ] [ str props.Text ]
        | Default -> defaultButton [ OnClick handleClick ] [ str props.Text ]

let render getProps = ofType<SubmitButton<'a>, _, _> (getProps defaultProps) []
