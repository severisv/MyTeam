module Shared.Components.Links

open Fable.React
open Fable.React.Props
open Shared.Util

let editAnchor attr =
    a (attr |> Html.mergeClasses [Class "edit-link pull-right" ]) [ Icons.edit "Rediger" ]

let linkButton attr  =
    button (attr |> Html.mergeClasses [Class "link" ])

let editButton attr =
    button (attr |> Html.mergeClasses [ Class "edit-link pull-right" ]) [ Icons.edit "Rediger" ]

let closeButton attr =
    button (attr |> Html.mergeClasses [Class "edit-link pull-right" ]) [ Icons.close ]
