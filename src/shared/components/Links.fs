namespace MyTeam.Shared.Components

open Fable.Helpers.React
open Fable.Helpers.React.Props

[<AutoOpen>]
module LinkComponents =  
    let editLink href =
        a 
            [Href href;Class "edit-link pull-right"]
            [Icons.edit "Rediger"]        
            