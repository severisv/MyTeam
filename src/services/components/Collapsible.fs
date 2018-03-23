namespace MyTeam.Views

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine
open System

[<AutoOpen>]
module Collapsible =  
    type CollapseState =
        | Open
        | Collapsed
    let collapsible collapseState header children =
        let isCollapsed = collapseState = Collapsed
        let id = Guid.NewGuid() |> string

        div [] [
            div [_class "collapselink-parent"] [    
                a [ 
                    _class <| sprintf "collapse-link %s" (isCollapsed =? ("collapsed", ""))
                    attr "role" "button"
                    attr "data-toggle" "collapse"
                    _href <| sprintf "#%s" id
                    attr "aria-expanded" (string <| not isCollapsed)
                  ] 
                  header                           
            ]
            div [_id id;_class <| sprintf "collapse %s" (isCollapsed =? ("","in")) ] children                
        ]       

   