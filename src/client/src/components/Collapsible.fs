namespace Shared.Components

open Fable.React
open Fable.React.Props
open System

[<AutoOpen>]
module Collapsible =
    type CollapseState =
        | Open
        | Collapsed
    
    let collapsible collapseState header children =
        let isCollapsed = collapseState = Collapsed
        let id = Guid.NewGuid() |> string
        div [] [ div [ Class "collapselink-parent" ] 
                     [ a [ Class <| sprintf "collapse-link %s" (if isCollapsed then "collapsed"
                                                                else "")
                           HTMLAttr.Custom("role", "button")
                           HTMLAttr.Custom("data-toggle", "collapse")
                           Href <| sprintf "#%s" id
                           HTMLAttr.Custom("aria-expanded", (string <| not isCollapsed)) ] header ]
                 div [ Id id
                       Class <| sprintf "collapse %s" (if isCollapsed then ""
                                                       else "in") ] children ]
