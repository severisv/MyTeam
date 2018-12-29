module MyTeam.Client.Components.Checkbox

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Fable.PowerPack
open Fable.PowerPack.Fetch
open MyTeam
open MyTeam.Components
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Input

type CheckboxState =
    { CurrentValue : bool
      Error : bool }

type CheckboxProps =
    { Value : bool
      Url : string
      OnChange : bool -> unit }

type Checkbox(props) =
    inherit Component<CheckboxProps, CheckboxState>(props)
    
    do 
        base.setInitState({ Error = false
                            CurrentValue = props.Value })
    
    member this.handleChange isSelected =
            let props = this.props
            let state = this.state
            props.OnChange isSelected
            this.setState(fun state props -> { state with Error = false })
            promise { 
                let! res = postRecord props.Url { value = isSelected } []
                if not res.Ok then failwithf "Received %O from server: %O" res.Status res.StatusText
                let! value = res.json<CheckboxPayload>()
                this.setState(fun state props -> { state with CurrentValue = isSelected })
                props.OnChange isSelected
            }
            |> Promise.catch(fun e -> 
                   Browser.console.error <| sprintf "%O" e
                   this.setState(fun state props -> { state with Error = true })
                   props.OnChange state.CurrentValue)
            |> Promise.start

    override this.render() =
        let state = this.state

        span [ Class "input-checkbox" ] [ input 
                                              [ Class "form-control"
                                                Type "checkbox"
                                                Checked state.CurrentValue
                                                OnChange(fun input -> this.handleChange input.Checked) ]
                                          (if this.state.Error then Labels.error
                                           else empty) ]

let render model = ofType<Checkbox, _, _> model []
