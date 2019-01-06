module MyTeam.Shared.Components.Labels

open Fable.Helpers.React
open Fable.Helpers.React.Props

let error =
    span [ Class "label label-danger"
           Title "Det oppstod en feil" ] [ i [ Class "fa fa-exclamation-triangle" ] [] ]

let success = span [ Class "label label-success" ] [ i [ Class "fa fa-check" ] [] ]
