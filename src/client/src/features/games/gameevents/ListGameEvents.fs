module Client.Features.Games.ListEvents

open Shared.Util
open Feliz
open Shared
open System
open Thoth.Json
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Client.Util
open Client.Features.Games.Common

let listGameEventsId = "list-game-events"

type Props = { GameId: Guid }

[<ReactComponent>]
let ListGameEvents (props: Props) =

    let (events, setEvents) =
        React.useState<List<GameEvent> option> (None)


    let (squad, setSquad) =
        React.useState<List<Member> option> (None)

    let (allPlayers, setAllPlayers) =
        React.useState<List<Member> option> (None)


    let fetchEvents () =
        Http.get
            $"/api/games/{props.GameId}/events"
            Decode.Auto.fromString<GameEvent list>
            { OnSuccess = Some >> setEvents
              OnError = printf "%O" }

    let fetchSquad () =
        Http.get
            $"/api/games/{props.GameId}/squad"
            Decode.Auto.fromString<Member list>
            { OnSuccess = Some >> setSquad
              OnError = printf "%O" }

    let fetchAllPlayers () =
        Http.get
            $"/api/members/compact"
            Decode.Auto.fromString<Member list>
            { OnSuccess = Some >> setAllPlayers
              OnError = printf "%O" }

    React.useEffect (fetchEvents, [||])
    React.useEffect (fetchAllPlayers, [||])
    React.useEffect (fetchSquad, [||])

    match allPlayers with
    | Some allPlayers ->
        Html.div [
            prop.children [
                Html.div [
                    prop.style [
                        style.display.flex
                        style.flexDirection.column
                        style.alignItems.center
                    ]
                    prop.children [
                        Html.div [
                            prop.className "gameEvents u-fade-in-on-enter"
                            prop.children (
                                match events with
                                | Some events -> Html.div (events |> List.map (renderEvent allPlayers))
                                | None -> Icons.spinner
                            )
                        ]
                        (match squad with
                         | Some squad when not squad.IsEmpty ->
                             Html.div [
                                 prop.className "u-fade-in-on-enter"
                                 prop.style [
                                     style.display.flex
                                     style.flexDirection.column
                                     style.alignItems.center
                                     style.marginTop 30
                                 ]
                                 prop.children [
                                     Html.h5 "Tropp"
                                     Html.ul [
                                         prop.className "game-squadList"
                                         prop.children (
                                             squad
                                             |> List.map (fun p ->
                                                 Html.li [
                                                     Icons.player ""
                                                     Html.text " "
                                                     playerLink <| Some p
                                                 ])
                                         )
                                     ]
                                 ]
                             ]
                         | Some _ -> Html.text ""
                         | None -> Icons.spinner)
                    ]
                ]
            ]
        ]
    | None -> Html.text ""

ReactHelpers.render2 Decode.Auto.fromString<Props> listGameEventsId ListGameEvents
