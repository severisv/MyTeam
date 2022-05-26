module Client.Components.AutoSync.Text

open Fable.React
open Fable.React.Props
open Fable.Import
open Shared.Components
open Shared.Components.Input
open Shared.Components.Base
open Client.Util
open Fetch.Types
open Feliz
open Shared.Util
open Thoth.Json

type State =
    { IsPosting: bool
      IsTouched: bool
      Error: bool }


type TextType =
    | String
    | Number

type Props =
    { Value: string
      Url: string
      Type: TextType
      OnChange: (string -> unit) option }

let componentId = "text-auto-sync"

[<ReactComponent>]
let Element (props: Props) =

    let initialState =
        { Error = false
          IsTouched = false
          IsPosting = false }
#if FABLE_COMPILER
    let mutable timeout = Browser.Dom.window.setTimeout (ignore, 0, [])

    let debounce fn wait =
        Browser.Dom.window.clearTimeout timeout
        timeout <- Browser.Dom.window.setTimeout (fn, wait, [])


    let (state, setState) = React.useState initialState

    let handleChange value =
        debounce
            (fun () ->
                setState
                    { state with
                        Error = false
                        IsTouched = true
                        IsPosting = true }

                promise {
                    let payload: StringPayload = { Value = value }
                    let! res = Http.sendRecord HttpMethod.PUT props.Url payload []

                    if not res.Ok then
                        failwithf "Received %O from server: %O" res.Status res.StatusText

                    setState (
                        { state with
                            IsPosting = false
                            Error = false }
                    )

                    props.OnChange |> Option.iter (fun fn -> fn value)
                }
                |> Promise.catch (fun e ->
                    Browser.Dom.console.error (sprintf "%O" e)

                    setState
                        { state with
                            Error = true
                            IsPosting = false })
                |> Promise.start)
            750
#else
    let handleChange _ = ()
    let state = initialState

#endif



    div [ Class $"input-text {componentId}" ] [
        input [
            Class "form-control"
            Type "text"
            DefaultValue props.Value
            OnChange (fun input ->
                let value = input.Value

                match props.Type with
                | Number when
                    not
                        (
                            Shared.Number.isNumber input.Value
                            || System.String.IsNullOrEmpty value
                        )
                    ->
                    ()
                | _ -> handleChange value)
        ]
        (match state with
         | { IsPosting = true } -> Icons.spinner
         | { Error = true } -> Icons.warning
         | { IsTouched = true } -> Icons.check
         | _ -> empty)
    ]


ReactHelpers.hydrateComponent componentId Decode.Auto.fromString<Props> Element
