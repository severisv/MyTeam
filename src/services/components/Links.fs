namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module LinkComponents =  
    let editLink href =
        a 
            [_href href;_class "edit-link pull-right"]
            [ Icons.edit "Rediger" ]        