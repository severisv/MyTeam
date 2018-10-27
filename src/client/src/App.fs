module MyTeam.Client.EditSeason

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.Import.Browser

let init() =
    let element = div [ Class "mt-main" ] []
    ReactDom.render(element, document.getElementById("main"))
