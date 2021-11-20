module Client.Features.Games.Form


open Client.Components

open Fable.React
open Fable.React.Props
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Datepicker
open Shared.Components.Forms
open Shared.Domain
open Shared.Util
open Shared.Util.ReactHelpers
open Shared.Validation
open System
open Thoth.Json

type State =
    { Opponent: string
      Date: DateTime option
      Time: string
      Location: string
      Team: Guid
      GameType: GameType
      Description: string
      IsHomeGame: bool }


type Props =
    { Teams: Team list
      GameTypes: GameType list
      Game: Game option }

[<CLIMutable>]
type AddGame =
    { Id: Guid option
      Opponent: string
      Date: DateTime
      Time: TimeSpan
      Location: string
      Team: Guid
      GameType: GameType
      Description: string
      IsHomeGame: bool }


let containerId = "game-form"


let element =
    FunctionComponent.Of (fun (props: Props) ->
        let state =
            Hooks.useState<State> (
                props.Game
                |> Option.map (fun game ->
                    { Date = Some game.DateTime.Date
                      Opponent = game.Opponent
                      Time = Date.formatTime game.DateTime
                      Location = game.Location
                      Team = game.Team.Id
                      GameType = game.Type
                      Description = game.Description
                      IsHomeGame = game.IsHomeTeam })
                |> Option.defaultValue
                    { Date = None
                      Opponent = ""
                      Time = ""
                      Location = ""
                      Team =
                        props.Teams
                        |> List.map (fun t -> t.Id)
                        |> List.head
                      GameType = props.GameTypes |> List.head
                      Description = ""
                      IsHomeGame = true }
            )

        let errorState = Hooks.useState<string option> None

        let setFormValue (v: State -> State) = state.update v
        let state = state.current

        let validation =
            Map [ "Date", validate "Dato" state.Date [ isSome ]
                  "Opponent", validate "Motstander" state.Opponent [ isRequired ]
                  "Time", validate "Klokkeslett" state.Time [ isRequired; isTimeString ]
                  "Location", validate "Sted" state.Location [ isRequired ] ]

        let formLayout = Horizontal 2

        fragment [] [
            props.Game
            => fun game ->
                Modal.modal
                    { OpenButton =
                        fun handleOpen ->
                            div [ ClassName "clearfix"
                                  Style [ MarginBottom "1em" ] ] [
                                btn [ ClassName "pull-right"
                                      Danger
                                      OnClick handleOpen ] [
                                    Icons.delete
                                ]
                            ]
                      Content =
                        fun handleClose ->
                            div [] [
                                h4 [] [
                                    str
                                    <| sprintf "Er du sikker på at du vil slette '%s'?" game.Name
                                ]
                                div [ Class "text-center" ] [
                                    br []
                                    Send.sendElement (fun o ->
                                        { o with
                                            SendElement = btn, [ Danger; Lg ], [ str "Slett" ]
                                            SentElement = span, [], []
                                            Endpoint = Send.Delete <| sprintf "/api/games/%O" game.Id
                                            OnSubmit =
                                                Some(
                                                    !>handleClose
                                                    >> (fun _ -> Browser.Dom.window.location.replace "/intern")
                                                ) })
                                    btn [ Lg; OnClick !>handleClose ] [
                                        str "Avbryt"
                                    ]
                                ]
                            ] }
            errorState.current => Alerts.danger
            form [ formLayout ] [
                div [ Class "form-group" ] [
                    label [ Class "control-label col-sm-2" ] [
                        Icons.team "Lag"
                    ]
                    div [ Class "col-sm-7" ] [
                        selectInput
                            [ Value state.Team
                              OnChange (fun e ->
                                  let id = e.Value
                                  setFormValue (fun form -> { form with Team = Guid.Parse id })) ]
                            (props.Teams
                             |> List.map (fun p -> { Name = p.Name; Value = p.Id }))
                    ]
                    checkboxInput [ Class "col-sm-3" ] [ str "Hjemme" ] state.IsHomeGame (fun value ->
                        setFormValue (fun form -> { form with IsHomeGame = value }))
                ]


                formRow
                    [ formLayout ]
                    [ Icons.users "Motstander" ]
                    [ textInput [ Validation validation.["Opponent"]
                                  OnChange (fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Opponent = value }))
                                  Placeholder "Mercantile SFK"
                                  Value state.Opponent ] ]

                formRow
                    [ formLayout ]
                    [ Icons.calendar "Dato" ]
                    [ dateInput [ Validation validation.["Date"]
                                  Value state.Date
                                  OnDateChange(fun date -> setFormValue (fun form -> { form with Date = date })) ] ]
                formRow
                    [ formLayout ]
                    [ Icons.clock ]
                    [ textInput [ Validation validation.["Time"]
                                  OnChange (fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Time = value }))
                                  Placeholder "18:30"
                                  Value state.Time ] ]

                formRow
                    [ formLayout ]
                    [ Icons.mapMarker "Sted" ]
                    [ textInput [ Validation validation.["Location"]
                                  OnChange (fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Location = value }))
                                  Placeholder "Ekeberg 2"
                                  Value state.Location ] ]

                formRow
                    [ formLayout ]
                    [ Icons.trophy "Turnering" ]
                    [ selectInput
                          [ OnChange (fun e ->
                                let s = e.Value

                                setFormValue (fun form ->
                                    { form with GameType = (Enums.fromString<GameType> typedefof<GameType> s) })) ]
                          (props.GameTypes
                           |> List.map (fun t -> { Name = string t; Value = t })) ]




                formRow
                    [ formLayout ]
                    [ Icons.description ]
                    [ textInput [ Validation []
                                  OnChange (fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Description = value }))
                                  Placeholder "Oppmøte 20 minutter før"
                                  Value state.Description ] ]



                formRow
                    [ formLayout; Style [ MarginBottom 0 ] ]
                    []
                    [ Send.sendElement (fun o ->
                          { o with
                              IsDisabled =
                                  validation
                                  |> Map.toList
                                  |> List.map (fun (_, v) -> v)
                                  |> List.concat
                                  |> List.exists (function
                                      | Error e -> true
                                      | _ -> false)
                              SendElement = btn, [ ButtonSize.Normal; Primary ], [ str "Lagre" ]
                              SentElement = btn, [ ButtonSize.Normal; Success ], [ str "Lagret" ]
                              Endpoint =
                                  match props.Game with
                                  | Some game ->
                                      Send.Put(
                                          sprintf "/api/games/%O" game.Id,
                                          Some (fun () ->
                                              Encode.Auto.toString (
                                                  0,
                                                  { Id = Some game.Id
                                                    Opponent = state.Opponent
                                                    Date = state.Date.Value
                                                    Time = (Date.tryParseTime state.Time).Value
                                                    Location = state.Location
                                                    Team = state.Team
                                                    GameType = state.GameType
                                                    Description = state.Description
                                                    IsHomeGame = state.IsHomeGame }
                                              ))
                                      )
                                  | None ->
                                      Send.Post(
                                          sprintf "/api/games",
                                          Some (fun () ->
                                              Encode.Auto.toString (
                                                  0,
                                                  { Id = None
                                                    Opponent = state.Opponent
                                                    Date = state.Date.Value
                                                    Time = (Date.tryParseTime state.Time).Value
                                                    Location = state.Location
                                                    Team = state.Team
                                                    GameType = state.GameType
                                                    Description = state.Description
                                                    IsHomeGame = state.IsHomeGame }
                                              ))
                                      )

                              OnSubmit =
                                  Some (fun res ->
                                      Decode.Auto.fromString<AddGame> res
                                      |> function
                                          | Ok game ->
                                              Browser.Dom.window.location.replace
                                              <| sprintf "/kamper/%O" game.Id

                                              ()

                                          | Error e -> errorState.update (Some e)) }) ]
            ]
        ])

hydrate2 containerId Decode.Auto.fromString<Props> element
