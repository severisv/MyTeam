module MyTeam.Games.Pages.Common

open Giraffe.ViewEngine
open Fable.React.Props
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open System
open Shared.Components


let gameDetails game =
    div [ _class "game-details-container" ] [
        div [ _class "game-details" ] [
            div [] [
                !!(Icons.calendar "")
                (game.DateTime.Year < DateTime.Now.Year
                 =? (Date.formatLong, Date.format))
                    game.DateTime
                |> encodedText
            ]
            div [] [
                !!(Icons.time "Tidspunkt")
                game.DateTime |> Date.formatTime |> encodedText
            ]

            div [] [
                !!(Icons.gameType game.Type)
                encodedText <| string game.Type
            ]
            div [] [
                a [ _href $"""https://www.google.com/maps/search/{game.Location.Replace(" kg", " kunstgress").Replace(" ", "+")}"""
                    _target "_blank"
                    _style "color:inherit;display:flex;align-items:center;" ] [
                    !!(Icons.mapMarker "Sted")
                    encodedText game.Location
                ]


            ]
        ]


    ]
