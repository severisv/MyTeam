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


let gameEventTypes =
    let cases =
        FSharp.Reflection.FSharpType.GetUnionCases(typeof<GameEventType>)

    [ for c in cases do
          yield FSharp.Reflection.FSharpValue.MakeUnion(c, [||]) :?> GameEventType ]

type GameEventForm =
    { Type: GameEventType
      PlayerId: PlayerId option
      AssistedById: PlayerId option }


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




    let addPlayer =
        function
        | Some player ->
            togglePlayer player Add ()
            setPlayerToAdd None
        | None -> ()



    let removeEvent (e: GameEvent) _ =
        promise {
            let! res = Http.sendRecord HttpMethod.POST $"/api/games/{props.GameId}/events/{e.Id}/delete" {|  |} []

            if not res.Ok then
                failwithf "Received %O from server: %O" res.Status res.StatusText

            setEvents (
                events
                |> Option.map (fun events -> events |> List.filter (fun p -> p.Id <> e.Id))
            )
        }
        |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
        |> Promise.start



    let defaultGameEvent =
        { AssistedById = None
          PlayerId = None
          Type = Mål }

    let (gameEventToAdd, setGameEventToAdd) =
        React.useState<GameEventForm> (defaultGameEvent)

    let addEvent _ =
        let gameEventToAdd =
            match gameEventToAdd with
            | { Type = ``Rødt kort``
                PlayerId = None }
            | { Type = ``Gult kort``
                PlayerId = None } ->
                { gameEventToAdd with
                    PlayerId =
                        (squad
                         |> Option.defaultValue []
                         |> List.tryHead
                         |> Option.map (fun p -> p.Id)) }
            | _ -> gameEventToAdd

        promise {
            let! res = Http.sendRecord HttpMethod.POST $"/api/games/{props.GameId}/events" gameEventToAdd []


            if not res.Ok then
                failwithf "Received %O from server: %O" res.Status res.StatusText

            let! text = res.text ()

            let ge =
                match Decode.Auto.fromString<GameEvent> text with
                | Ok ge -> ge
                | _ -> failwithf "Could not decode %O" text

            setEvents (
                events
                |> Option.map (fun events -> events @ [ ge ])
            )

            setGameEventToAdd defaultGameEvent
        }
        |> Promise.catch (fun e -> Browser.Dom.console.error (sprintf "%O" e))
        |> Promise.start

    match allPlayers with
    | Some allPlayers ->
        let selectablePlayers =
            allPlayers
            |> List.filter (fun p ->
                (squad |> Option.defaultValue [])
                |> List.contains p
                |> not)

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
                                | Some events ->
                                    Html.div (
                                        (events
                                         |> List.map (fun e ->
                                             Html.div [
                                                 prop.style [
                                                     style.display.flex
                                                     style.justifyContent.spaceBetween
                                                     style.alignItems.center
                                                 ]
                                                 prop.children [
                                                     renderEvent allPlayers e
                                                     Html.a [
                                                         prop.className "link link--red"
                                                         prop.style [
                                                             style.marginLeft 20
                                                             style.fontSize 21
                                                         ]
                                                         prop.onClick <| removeEvent e
                                                         prop.children [ Icons.remove ]
                                                     ]
                                                 ]
                                             ]))
                                        @ [ form [ Horizontal 12
                                                   Style [ MarginTop 30; MarginBottom 30 ] ] [
                                                formRow
                                                    [ Horizontal 3 ]
                                                    [ Html.text "Hendelse" ]
                                                    [ selectInput
                                                          [ OnChange (fun (e) ->
                                                                let value =
                                                                    match e.Value with
                                                                    | "Mål" -> Mål
                                                                    | "Gult kort" -> ``Gult kort``
                                                                    | "Rødt kort" -> ``Rødt kort``
                                                                    | _ -> failwithf "Unknown event type: %O" e.Value

                                                                setGameEventToAdd { gameEventToAdd with Type = value }) ]
                                                          ((gameEventTypes
                                                            |> List.map (fun p -> { Name = string p; Value = p }))) ]
                                                formRow
                                                    [ Horizontal 3 ]
                                                    [ Html.text "Hvem" ]
                                                    [ selectInput
                                                          [ OnChange (fun (e) ->
                                                                let id = e.Value
                                                                setGameEventToAdd { gameEventToAdd with PlayerId = id |> Guid.tryParse }) ]
                                                          (if gameEventToAdd.Type = Mål then
                                                               [ { Name = "( Selvmål )"; Value = "" } ]
                                                           else
                                                               []
                                                           @ (squad
                                                              |> Option.defaultValue []
                                                              |> List.map (fun p -> { Name = p.Name; Value = p.Id }))) ]

                                                (if gameEventToAdd.Type = Mål then
                                                     formRow
                                                         [ Horizontal 3 ]
                                                         [ Html.text "Assist" ]
                                                         [ selectInput
                                                               [ OnChange (fun (e) ->
                                                                     let id = e.Value

                                                                     setGameEventToAdd
                                                                         { gameEventToAdd with AssistedById = id |> Guid.tryParse }) ]
                                                               ([ { Name = "( Ingen )"; Value = "" } ]
                                                                @ (squad
                                                                   |> Option.defaultValue []
                                                                   |> List.filter (fun p ->
                                                                       p.Id
                                                                       <> (gameEventToAdd.PlayerId
                                                                           |> Option.defaultValue Guid.Empty))
                                                                   |> List.map (fun p -> { Name = p.Name; Value = p.Id }))) ]
                                                 else
                                                     Html.text "")
                                                formRow
                                                    [ Horizontal 3 ]
                                                    []
                                                    [ btn [ Primary
                                                            OnClick(addEvent)

                                                            ] [
                                                          Fable.React.Helpers.str "Legg til"

                                                      ] ]
                                            ]



                                         ]
                                    )
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

                                             btn [ Primary
                                                   OnClick (fun _ ->
                                                       let playerToAdd =
                                                           allPlayers
                                                           |> List.tryFind (fun p -> Some p.Id = (playerToAdd |> Option.bind Guid.tryParse))
                                                           |> function
                                                               | Some v -> Some v
                                                               | None -> (selectablePlayers |> List.tryHead)

                                                       addPlayer playerToAdd) ] [
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
