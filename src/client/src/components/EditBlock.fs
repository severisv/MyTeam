module Client.Components.EditBlock

open Fable.Helpers.React
open Fable.Import
open Fable.Import.React
open Shared.Components



type State =
    { IsInEditMode : bool }

type Props =
    { Render : bool -> React.ReactElement }

let toggleEditMode setState _ =
    setState (fun state props -> { state with IsInEditMode = not state.IsInEditMode })

type EditBlock(props) =
    inherit Component<Props, State>(props)
    do base.setInitState ({ IsInEditMode = false })
    override this.render() =
        let toggleEditMode = toggleEditMode this.setState
        let state = this.state
        fragment [] [ (if state.IsInEditMode then closeButton toggleEditMode
                       else editButton toggleEditMode)
                      this.props.Render(this.state.IsInEditMode) ]

let render model = ofType<EditBlock, _, _> model []
