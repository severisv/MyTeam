module Client.Features.Trainings.Form


open Client.Components

open Fable.React
open Fable.React.Props
open Shared
open Shared.Components
open Shared.Components.Base
open Shared.Components.Datepicker
open Shared.Components.Forms
open Shared.Components.Links
open Shared.Domain
open Shared.Util
open Shared.Util.ReactHelpers
open Shared.Validation
open System
open Thoth.Json

type State =
    { Date: DateTime option
      Time: string
      Location: string
      Teams: Guid list
      Description: string }

type RecurringState =
    { IsRecurring: bool
      Until: DateTime option
      Dates: DateTime list }

type Props =
    { Teams: Team list
      Training: Training option }

[<CLIMutable>]
type AddOrUpdateTraining =
    { Id: Guid option
      Date: DateTime
      Time: TimeSpan
      Location: string
      Teams: Guid list
      Description: string }


let containerId = "training-form"


let element =
    FunctionComponent.Of(fun (props: Props) ->
        let state =
            Hooks.useState<State>
                (props.Training
                 |> Option.map (fun training ->
                     { Date = Some training.DateTime.Date
                       Time = Date.formatTime training.DateTime
                       Location = training.Location
                       Teams = training.Teams
                       Description = training.Description })
                 |> Option.defaultValue
                     { Date = None
                       Time = ""
                       Location = ""
                       Teams = props.Teams |> List.map (fun t -> t.Id)
                       Description = "" })

        let errorState = Hooks.useState<string option> None

        let setFormValue (v: State -> State) = state.update v
        let state = state.current

        let recurringState =
            Hooks.useState<RecurringState>
                { IsRecurring = false
                  Until = None
                  Dates = [] }

        let handleRecurringUntilChange from (until: DateTime option) =
            let dates =
                match (from, until) with
                | (Some from, Some until) when from < until ->
                    [ 0 .. (until - from).Days ]
                    |> List.filter (fun offset -> offset % 7 = 0)
                    |> List.map (float >> from.AddDays)
                | _ -> []

            recurringState.update (fun form ->
                { form with
                      Until = until
                      Dates = dates })

        let validation =
            Map [ "Date", validate "Dato" state.Date [ isSome ]
                  "Time", validate "Klokkeslett" state.Time [ isRequired; isTimeString ]
                  "UntilDate",
                  validate
                      "Til dato"
                      (if recurringState.current.IsRecurring then recurringState.current.Until else None)
                      [ dateIsAfter ("dato for treningen", state.Date) ]
                  "Location", validate "Sted" state.Location [ isRequired ] ]

        let formLayout = Horizontal 2

        fragment [] [
            props.Training
            => fun training ->
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
                                      <| sprintf "Er du sikker på at du vil slette '%s'?" training.Name
                                  ]
                                  div [ Class "text-center" ] [
                                      br []
                                      Send.sendElement (fun o ->
                                          { o with
                                                SendElement = btn, [ Danger; Lg ], [ str "Slett" ]
                                                SentElement = span, [], []
                                                Endpoint =
                                                    Send.Delete
                                                    <| sprintf "/api/trainings/%O" training.Id
                                                OnSubmit =
                                                    Some
                                                        (!>handleClose
                                                         >> (fun _ -> Browser.Dom.window.location.replace "/intern")) })
                                      btn [ Lg; OnClick !>handleClose ] [
                                          str "Avbryt"
                                      ]
                                  ]
                              ] }
            errorState.current => Alerts.danger
            form [ formLayout ] [
                formRow
                    [ formLayout ]
                    [ Icons.team "Lag" ]
                    [ multiSelect
                        { OnChange = (fun teams -> setFormValue (fun form -> { form with Teams = teams }))
                          Options =
                              (props.Teams
                               |> List.map (fun p -> { Name = p.Name; Value = p.Id }))
                          Values = state.Teams } ]


                formRow
                    [ formLayout ]
                    [ Icons.calendar "Dato" ]
                    [ dateInput [ Validation validation.["Date"]
                                  Value state.Date
                                  OnDateChange(fun date ->
                                      handleRecurringUntilChange date recurringState.current.Until
                                      setFormValue (fun form -> { form with Date = date })) ] ]
                formRow
                    [ formLayout ]
                    [ Icons.clock ]
                    [ textInput [ Validation validation.["Time"]
                                  OnChange(fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Time = value }))
                                  Placeholder "18:30"
                                  Value state.Time ] ]

                formRow
                    [ formLayout ]
                    [ Icons.mapMarker "Sted" ]
                    [ textInput [ Validation validation.["Location"]
                                  OnChange(fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Location = value }))
                                  Placeholder "Valle Hovin"
                                  Value state.Location ] ]


                formRow
                    [ formLayout ]
                    [ Icons.description ]
                    [ textInput [ Validation []
                                  OnChange(fun e ->
                                      let value = e.Value
                                      setFormValue (fun form -> { form with Description = value }))
                                  Placeholder "Beskrivelse"
                                  Value state.Description ] ]

                props.Training.IsNone
                &? div [ Class "form-group" ] [
                    label [ Class "control-label col-sm-2" ] [
                        Icons.refresh "Gjentakende"
                    ]
                    checkboxInput [ Class "col-sm-1" ] [] recurringState.current.IsRecurring (fun value ->
                        recurringState.update (fun form -> { form with IsRecurring = value }))
                    recurringState.current.IsRecurring
                    &? div [ Class "col-sm-9" ] [
                        dateInput [ Value recurringState.current.Until
                                    Validation validation.["UntilDate"]
                                    OnDateChange(handleRecurringUntilChange state.Date) ]
                       ]
                   ]
                div [ Class "form-group" ] [
                    div [ Class "col-sm-10 col-sm-offset-2" ] [
                        recurringState.current.IsRecurring
                        &? fr
                            []
                               (recurringState.current.Dates
                                |> List.map (fun date ->
                                    div [ Style [ Display DisplayOptions.Flex
                                                  JustifyContent "space-between"
                                                  FontSize "1.25em"
                                                  LineHeight "2em" ] ] [
                                        div [] [
                                            span [ Style [ MarginRight "0.8em" ] ] [
                                                Icons.training ""
                                            ]
                                            str <| Date.format date
                                        ]

                                        linkButton [ OnClick
                                                     <| fun _ ->
                                                         recurringState.update (fun state ->
                                                             { state with
                                                                   Dates = state.Dates |> List.filter ((<>) date) }) ] [
                                            Icons.delete
                                        ]
                                    ]))
                    ]
                ]
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
                              SendElement = btn, [ Normal; Primary ], [ str "Lagre" ]
                              SentElement = btn, [ Normal; Success ], [ str "Lagret" ]
                              Endpoint =
                                  match props.Training with
                                  | Some training ->
                                      Send.Put
                                          (sprintf "/api/trainings/%O" training.Id,
                                           Some(fun () ->
                                               Encode.Auto.toString
                                                   (0,
                                                    { Id = Some training.Id
                                                      Date = state.Date.Value
                                                      Time = (Date.tryParseTime state.Time).Value
                                                      Location = state.Location
                                                      Teams = state.Teams
                                                      Description = state.Description })))
                                  | None ->
                                      Send.Post
                                          (sprintf "/api/trainings",
                                           Some(fun () ->
                                               Encode.Auto.toString
                                                   (0,
                                                    match recurringState.current with
                                                    | { Dates = dates; IsRecurring = true } when not dates.IsEmpty ->
                                                        dates
                                                    | _ -> [ state.Date.Value ]
                                                    |> List.map (fun date ->
                                                        { Id = None
                                                          Date = date
                                                          Time = (Date.tryParseTime state.Time).Value
                                                          Location = state.Location
                                                          Teams = state.Teams
                                                          Description = state.Description }))))

                              OnSubmit = Some(fun _ -> Browser.Dom.window.location.replace "/intern") }) ]
            ]
        ])

hydrate2 containerId Decode.Auto.fromString<Props> element
