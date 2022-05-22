module Client.Components.AutoSync.Checkbox

open Client.Util
open Fable.React
open Fable.React.Props
open Fable.Import
open Fetch.Types
open Shared.Components
open Shared.Components.Input
open Shared.Components.Base
open Feliz
open Shared.Util

type State = { CurrentValue: bool; Error: bool }

type Props =
    { Value: bool
      Url: string
      OnChange: (bool -> unit) option }

let internal componentId = "checkbox-auto-sync"

[<ReactComponent>]
let Element (props: Props) =

    let initialState =
        { CurrentValue = props.Value
          Error = false }

#if FABLE_COMPILER
    let (state, setState) = React.useState initialState

    let onChange value =
        props.OnChange
        |> Option.iter (fun onChange -> onChange value)

    let handleChange isSelected =

        onChange isSelected
        setState ({ state with Error = false })

        promise {
            let! res = Http.sendRecord HttpMethod.POST props.Url { value = isSelected } []

            if not res.Ok then
                failwithf "Received %O from server: %O" res.Status res.StatusText

            setState (
                { state with
                    CurrentValue = isSelected
                    Error = false }
            )

            onChange isSelected
        }
        |> Promise.catch (fun e ->
            Browser.Dom.console.error (sprintf "%O" e)
            setState ({ state with Error = true })
            onChange state.CurrentValue)
        |> Promise.start
#else
    let state = initialState
    let handleChange _ = ()
#endif

    span [ Class $"input-checkbox {componentId}" ] [
        input [
            Class "form-control"
            Type "checkbox"
            Checked state.CurrentValue
            OnChange(fun input -> handleChange input.Checked)
        ]
        (if state.Error then
             Labels.error
         else
             empty)
    ]

ReactHelpers.hydrateComponent componentId Thoth.Json.Decode.Auto.fromString<Props> Element
