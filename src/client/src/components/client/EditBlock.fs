module Client.Components.EditBlock

open Fable.React
open Shared.Components.Links

type State =
    { IsInEditMode : bool }

type Props =
    { Render : bool -> ReactElement }

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
