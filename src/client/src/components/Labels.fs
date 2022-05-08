module Shared.Components.Labels

open Fable.React
open Fable.React.Props

let error =
    span [ Class "label label-danger"
           Title "Det oppstod en feil" ] [
        Icons.warning
    ]

let success =
    span [ Class "label label-success" ] [
        Icons.check
    ]


type LabelColor =
    | Gray
    | DarkBlue
    | Green
    | LightBlue
    | Yellow
    | Red

let label (color: LabelColor) children =
    let color =
        match color with
        | Gray -> "default"
        | DarkBlue -> "primary"
        | Green -> "success"
        | LightBlue -> "info"
        | Yellow -> "warning"
        | Red -> "danger"

    span [ Class $"label label-{color}" ] children
