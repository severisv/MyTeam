namespace MyTeam.Components

open Fable.Helpers.React
open Fable.Helpers.React.Props
open MyTeam.Shared.Util

[<AutoOpen>]
module DivComponents =
    let whitespace = str " "
    let empty = str ""
    
    let tooltip message attr children =
        a (Html.mergeClasses [ Class "mt-popover"
                               Href "javascript:void(0);"
                               HTMLAttr.Custom("data-container", "body")
                               HTMLAttr.Custom("data-content", message)
                               HTMLAttr.Custom("data-placement", "right")
                               HTMLAttr.Custom("data-toggle", "popover") ] attr) children
    
    let (=>) optn fn =
        optn
        |> Option.map fn
        |> Option.defaultValue empty
    
    let (&?) shouldShow element =
        if shouldShow then element
        else empty
