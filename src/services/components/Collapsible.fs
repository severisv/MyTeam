namespace MyTeam.Views

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine
open System

[<AutoOpen>]
module Collapsible =  
    let collapsible isCollapsed header children =
        let id = Guid.NewGuid().ToString()

        div [] [
            div [_class "collapselink-parent"] [    
                a [ 
                    _class <| sprintf "collapse-link %s" (isCollapsed =? ("collapsed", ""))
                    attr "role" "button"
                    attr "data-toggle" "collapse"
                    _href <| sprintf "#%s" id
                    attr "aria-expanded" (str <| not isCollapsed)
                  ] 
                  header                           
            ]
            div [_id id;_class <| sprintf "collapse %s" (isCollapsed =? ("","in")) ] [ 
                    div [ _class "row" ] children
                ]
        ]       

   