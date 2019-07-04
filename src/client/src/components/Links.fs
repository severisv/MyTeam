namespace Shared.Components

open Fable.React
open Fable.React.Props
open Shared.Util

[<AutoOpen>]
module LinkComponents =
    let editLink href =
        a [ Href href
            Class "edit-link pull-right" ] [ Icons.edit "Rediger" ]
    
    let linkButton onClick children =
        button [ OnClick onClick
                 Class "link" ] children
    
    let linkButton2 attr  =
        button (attr |> Html.mergeClasses [Class "link" ])
    
    let editButton onClick =
        button [ OnClick onClick
                 Class "edit-link pull-right" ] [ Icons.edit "Rediger" ]
    
    let closeButton onClick =
        button [ OnClick onClick
                 Class "edit-link pull-right" ] [ Icons.close ]
