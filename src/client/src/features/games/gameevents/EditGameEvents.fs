module Client.Features.Games.EditEvents

open Shared.Util
open Feliz
open Shared
open System
open Thoth.Json
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Shared.Components.Forms
open Client.Util
open Client.Features.Games.Common
open Fetch
open Fable.React.Props
open Fable.React.Extensions

let editGameEventsId = "edit-game-events"


type Props = { GameId: Guid }

type AddOrRemove =
    | Add
    | Remove

[<ReactComponent>]
let EditGameEvents (props: Props) =

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



    let togglePlayer (player: Member) (addOrRemove: AddOrRemove) _ =
        match squad with
        | Some squad ->
            let value =
                match addOrRemove with
                | Add -> true
                | Remove -> false

            promise {
                let! res = Http.sendRecord HttpMethod.POST $"/api/games/{props.GameId}/squad/select/{player.Id}" {| value = value |} []

                if not res.Ok then
                    failwithf "Received %O from server: %O" res.Status res.StatusText

                let result =
                    match addOrRemove with
                    | Remove -> squad |> List.filter (fun p -> p.Id <> player.Id)
                    | Add ->
                        (squad @ [ player ])
                        |> List.sortByDescending (fun p -> p.FirstName)

                setSquad (Some result)
            }
            |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
            |> Promise.start

        | None -> ()


    let (playerToAdd, setPlayerToAdd) = React.useState<string option> (None)


    let selectablePlayers =
        allPlayers
        |> Option.defaultValue []
        |> List.filter (fun p ->
            (squad |> Option.defaultValue [])
            |> List.contains p
            |> not)

    let addPlayer _ =
        allPlayers
        |> Option.defaultValue []
        |> List.tryFind (fun p -> Some p.Id = (playerToAdd |> Option.bind Guid.tryParse))
        |> function
            | Some v -> Some v
            | None -> (selectablePlayers |> List.tryHead)
        |> function
            | Some player ->
                togglePlayer player Add ()
                setPlayerToAdd None
            | None -> ()


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
                         | Some squad ->
                             let squad = squad |> List.sortBy (fun p -> p.Name)

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
                                                     prop.style [
                                                         style.display.flex
                                                         style.justifyContent.spaceBetween
                                                         style.alignItems.center
                                                         style.marginBottom 6
                                                     ]
                                                     prop.children [
                                                         Html.div [
                                                             prop.children [
                                                                 Icons.player ""
                                                                 Html.text " "
                                                                 playerLink <| Some p
                                                             ]
                                                         ]
                                                         Html.a [
                                                             prop.className "link link--red"
                                                             prop.style [ style.marginLeft 20 ]
                                                             prop.onClick <| togglePlayer p Remove
                                                             prop.children [ Icons.remove ]
                                                         ]
                                                     ]
                                                 ])
                                         )
                                     ]
                                     Html.div [
                                         prop.style [
                                             style.width (length.percent 100)
                                         ]
                                         prop.children [

                                             selectInput
                                                 [ Style [ MarginBottom "15px" ]
                                                   OnChange (fun (e) ->
                                                       let id = e.Value
                                                       setPlayerToAdd <| Some id) ]
                                                 (selectablePlayers
                                                  |> List.map (fun p -> { Name = p.Name; Value = p.Id }))

                                             btn [ Primary; OnClick addPlayer ] [
                                                 Fable.React.Helpers.str "Legg til"

                                             ]
                                         ]
                                     ]
                                 ]
                             ]
                         | None -> Icons.spinner)
                    ]
                ]
            ]
        ]
    | None -> Html.text ""

ReactHelpers.render2 Decode.Auto.fromString<Props> editGameEventsId EditGameEvents
