module MyTeam.Client.SelectSquad

open MyTeam.Shared.Domain

open Fable.Helpers.React
open Fable.Import
open Fable.Import.Browser
open MyTeam.Shared



let init() =
    let element = str "Hello 🌍"


    ReactDom.render(element, document.getElementById(ClientViews.selectSquad))

init()
