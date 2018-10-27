module MyTeam.Shared.Components.Layout


open Fable.Helpers.React
open Fable.Helpers.React.Props
open MyTeam.Shared.Util

    
let mtMain attributes children = 
    div (attributes |> Html.mergeClasses [Class "mt-main"] ) children 

let sidebar attributes children = 
    div (attributes |> Html.mergeClasses [Class "mt-sidebar"]) children 

let block attributes children = 
    div (attributes |> Html.mergeClasses [Class "mt-container"]) children 
