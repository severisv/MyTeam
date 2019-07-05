module Client.Components.Modal

open Browser.Types
open Fable
open Fable.React
open Fable.React.Props
open Shared.Components.Links
open Shared.Components.Base
open Shared.Util

type State =
    { IsVisible : bool }

type Props =
    { OpenButton : (MouseEvent -> unit) -> React.ReactElement
      Content : (unit -> unit) -> React.ReactElement }

let openModal setState _ = setState (fun state props -> { state with IsVisible = true })
let closeModal setState _ = setState (fun state props -> { state with IsVisible = false })

let modal props = 
    ReactHelpers.komponent<Props,State>
        props
        { IsVisible = false }
        None
        (fun (props, state, setState) ->                        
            fragment [] [ props.OpenButton <| openModal setState
                          (if state.IsVisible then 
                               div [ Class "modal" ] 
                                   [ div [ Class "modal-overlay"
                                           OnClick <| closeModal setState ] []                                 
                                     div [ Class "modal-wrapper" ] 
                                         [ div [ Class "modal-window" ] 
                                               [ closeButton <| closeModal setState
                                                 props.Content <| closeModal setState ] ] ]
                           else empty) ]
        )
        