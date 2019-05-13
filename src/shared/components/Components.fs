module Shared.Components.Base

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Shared.Util
open ReactHelpers
open Fable.Core

let whitespace = str " "
let empty = str ""

[<Emit("jQuery('.' + $0).popover({ trigger: 'hover' })")>]
let apply (id: System.Guid): unit = failwith "JS only"

type State = {
    Id : System.Guid
}

let tooltip message attr children =    
    komponent<unit, State> () { Id = System.Guid.NewGuid() }
        (Some { ComponentDidMount = fun (_,state,_) -> apply(state.Id) })
        (fun (_,state,_) ->
             a (Html.mergeClasses
                         [ Class <| string state.Id
                           Href "javascript:void(0);"
                           HTMLAttr.Custom("data-container", "body")
                           HTMLAttr.Custom("data-content", message)
                           HTMLAttr.Custom("data-placement", "right")
                           HTMLAttr.Custom("data-toggle", "popover") ] attr) children)
       
let (=>) optn fn =
    optn
    |> Option.map fn
    |> Option.defaultValue empty

let (&?) shouldShow element =
    if shouldShow then element
    else empty


let (!>) fn =
    fun _ -> fn () 