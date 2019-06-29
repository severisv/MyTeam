module Shared.Components.Layout

open Fable.React
open Fable.React.Props
open Shared.Util

let mtMain attributes children =
    div (attributes |> Html.mergeClasses [ Class "mt-main" ]) children

let sidebar attributes children =
    div (attributes |> Html.mergeClasses [ Class "mt-sidebar" ]) children

let block attributes children =
    div (attributes |> Html.mergeClasses [ Class "mt-container" ]) children
