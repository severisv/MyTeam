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

let listGameEventsId = "list-game-events"


type Props = { GameId: Guid }


let playerLink (player: Member option) =
    player
    |> Option.map (fun player ->
        Html.a [
            prop.className "underline"
            prop.href $"/spillere/vis/{player.UrlName}"
            prop.title player.FullName
            prop.children [ Html.text player.Name ]
        ])

    |> Option.defaultValue (Html.text "")


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
                        style.marginTop 45
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
                         | Some squad ->
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
                         | None -> Icons.spinner)
                    ]
                ]
            ]
        ]
    | None -> Html.text ""

ReactHelpers.render2 Decode.Auto.fromString<Props> listGameEventsId ListGameEvents
