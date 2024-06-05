module MyTeam.Players.Pages.Common

open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open Shared.Components
open Shared.Components.Nav
open Shared.Domain.Members
open Fable.React
open Fable.React.Props

let listPlayersUrl =
    function
    | Aktiv -> sprintf "/spillere"
    | status -> sprintf "/spillere/%O" status |> Strings.toLower

let showPlayerUrl = Strings.toLower >> sprintf "/spillere/vis/%s"

let sidebar status (players: Member list) currentPlayerUrlName =
    sidebar [] [
        block [] [
            !!(navList
                { Header = "Kategori"
                  Items =
                    [ { Text =
                          [ (Icons.user "")
                            (string >> Fable.React.Helpers.str) " Aktive spillere" ]
                        Url = listPlayersUrl Aktiv }
                      { Text =
                          [ (i [ Class "flaticon-football50"
                                 Title "Trenere"
                                 Style [
                                     Height "14px"
                                     Width "14px"
                                     MarginLeft "-3px"
                                 ] ] [])
                            (string >> Fable.React.Helpers.str) " Trenere" ]
                        Url = listPlayersUrl PlayerStatus.Trener }
                      { Text =
                          [ Icons.game ""
                            (string >> Fable.React.Helpers.str) " Hall of Fame" ]
                        Url = listPlayersUrl Veteran } ]
                  Footer = None
                  IsSelected = never })
        ]
        block [] [
            !!(navList
                { Header =
                    match status with
                    | Veteran -> "Hall of Fame"
                    | PlayerStatus.Trener -> "Trenere"
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
