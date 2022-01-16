module Client.Features.Games.Common

open Shared.Util
open Feliz
open Shared
open System
open Thoth.Json
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Client.Util

let playerLink (player: Member option) =
    player
    |> Option.map (fun player ->
        Html.a [
            prop.className "underline"
            prop.href $"/spillere/vis/{player.UrlName}"
            prop.title player.FullName
            prop.children [ Html.text player.Name ]
        ])

    |> Option.defaultValue (Html.text "Selvmål")


let renderEvent (allPlayers: Member list) (gameEvent: GameEvent) =
    let player =
        allPlayers
        |> List.tryFind (fun p -> Some p.Id = gameEvent.PlayerId)

    let assistedBy =
        allPlayers
        |> List.tryFind (fun p -> Some p.Id = gameEvent.AssistedById)
        |> Option.map (fun assistedBy ->
            Html.span [
                Html.text " ( "
                Icons.assist "Assist"
                Html.text " "
                playerLink <| Some assistedBy
                Html.text ")"
            ])
        |> Option.defaultValue (Html.text "")



    Html.div [
        prop.className "gameEvent"
        prop.children [
            Html.span [
                prop.className "no-wrap"
                prop.children [
                    Icons.gameEvent gameEvent.Type
                    Html.text " "
                    playerLink player
                    assistedBy
                ]
            ]
        ]
    ]
