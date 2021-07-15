namespace MyTeam.Views

open Giraffe.ViewEngine

[<AutoOpen>]
module LayoutComponents =
    let mtMain attributes children =
        div ([ _class "mt-main" ] |> mergeAttributes attributes) children

    let sidebar attributes children =
        div
            ([ _class "mt-sidebar" ]
             |> mergeAttributes attributes)
            children

    let block attributes children =
        div
            ([ _class "mt-container" ]
             |> mergeAttributes attributes)
            children
