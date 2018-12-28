namespace MyTeam.Shared.Components

open Fable.Helpers.React
open Fable.Helpers.React.Props

[<AutoOpen>]
module LinkComponents =  
    let editLink href =
        a 
            [Href href; Class "edit-link pull-right"]
            [Icons.edit "Rediger"]        
            

    let editButton onClick =
        button 
            [OnClick onClick; Class "edit-link pull-right"]
            [Icons.edit "Rediger"]      

    let closeButton onClick =
        button 
            [OnClick onClick; Class "edit-link pull-right"]
            [Icons.close]                                

