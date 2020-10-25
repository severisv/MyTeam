module MyTeam.Players.Pages.Common

open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open Shared.Components
open Shared.Components.Nav
open Shared.Domain.Members


let listPlayersUrl =
    function
    | Aktiv -> sprintf "/spillere"
    | status -> sprintf "/spillere/%O" status

let showPlayerUrl =
    Strings.toLower >> sprintf "/spillere/vis/%s"

let sidebar status (players: Member list) currentPlayerUrlName =
    sidebar [] [
        block [] [
            !!(navList
                { Header = "Spillerkategori"
                  Items =
                      [ { Text = [ Icons.user ""; (string >> Fable.React.Helpers.str) " Aktive spillere" ]
                          Url = listPlayersUrl Aktiv }
                        { Text = [ Icons.game ""; (string >> Fable.React.Helpers.str) " Hall of Fame" ]
                          Url = listPlayersUrl Veteran } ]
                  Footer = None
                  IsSelected = never })
        ]
        block [] [
            !!(navList
                { Header =
                      match status with
                      | Veteran -> "Hall of Fame"
                      | status -> sprintf "%Oe Spillere" status
                  Items =
                      players
                      |> List.map (fun player ->
                          { Text = [ (string >> Fable.React.Helpers.str) player.Name ]
                            Url = showPlayerUrl player.UrlName })
                  Footer = None
                  IsSelected = (=) (showPlayerUrl currentPlayerUrlName) })
        ]
    ]
