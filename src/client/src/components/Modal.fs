module Client.Components.Modal

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.React
open Shared.Components
open Shared.Components.Base


type State =
    { IsVisible : bool }

type Props =
    { OpenButton : (React.MouseEvent -> unit) -> React.ReactElement
      Content : (React.MouseEvent -> unit) -> React.ReactElement }

let openModal setState _ = setState (fun state props -> { state with IsVisible = true })
let closeModal setState _ = setState (fun state props -> { state with IsVisible = false })

type Modal(props) =
    inherit Component<Props, State>(props)
    do base.setInitState ({ IsVisible = false })
    override this.render() =
        let state = this.state
        let opena = openModal this.setState
        fragment [] [ this.props.OpenButton <| openModal this.setState
                      (if state.IsVisible then 
                           div [ Class "modal" ] 
                               [ div [ Class "modal-overlay"
                                       OnClick <| closeModal this.setState ] []
                                 
                                 div [ Class "modal-wrapper" ] 
                                     [ div [ Class "modal-window" ] 
                                           [ closeButton <| closeModal this.setState
                                             props.Content <| closeModal this.setState ] ] ]
                       else empty) ]

let render model = ofType<Modal, _, _> model []
