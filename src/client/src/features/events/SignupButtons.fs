module Client.Events.SignupButtons

open Client.Components
open Client.Events
open Fable.Import
open Shared.Util.ReactHelpers
open Thoth.Json
open Fable.React
open Fable.React.Props
open Shared.Components
open Send
open Shared.Components.Base
open System

type Props =
    { HandleSignup: bool -> string -> unit
      Event: Event
      UserAttendance: Attendee option }

type BoolState = { Value: bool }

let element props =

    let userAttendance = props.UserAttendance

    let userIsAttending =
        userAttendance
        |> Option.map (fun ea -> ea.IsAttending)
        |> Option.defaultValue None

    let event = props.Event

    div [ Class "event-signupButtons" ] [
        div [ Class "event-col-1" ] []
        div [] [
            sendElement (fun o ->
                { o with
                    IsSent = userIsAttending
                    SentClass = None
                    SentIndicator = None
                    Spinner = None
                    OnSubmit = Some <| props.HandleSignup true
                    SendElement = btn, [ Lg ], [ str "Stiller" ]
                    SentElement = btn, [ Success; Lg ], [ str "Stiller" ]
                    Endpoint =
                        Put(
                            sprintf "/api/events/%O/signup" event.Id,
                            Some
                            <| fun () -> Encode.Auto.toString (0, { IsAttending = true })
                        ) })
            sendElement (fun o ->
                { o with
                    IsSent =
                        (userAttendance
                         |> Option.map (fun a -> a.IsAttending = Some false))
                    SentClass = None
                    Spinner = None
                    SentIndicator = None
                    IsDisabled =
                        props.UserAttendance.IsSome
                        && event.DateTime.ToUniversalTime()
                           <= DateTime.UtcNow.AddHours Event.allowedSignoffHours
                    OnSubmit = Some <| props.HandleSignup false
                    SendElement = btn, [ Lg ], [ str "Kan ikke" ]
                    SentElement = btn, [ Danger; Lg ], [ str "Kan ikke" ]
                    Endpoint =
                        Put(
                            sprintf "/api/events/%O/signup" event.Id,
                            Some
                            <| fun () -> Encode.Auto.toString (0, { IsAttending = false })
                        ) })

            komponent<unit, BoolState> () { Value = false } None (fun (props, state, setState) ->
                fr [] [
                    a [ Title "Beskjed til trenerne"
                        Class
                        <| sprintf "event-addMessage %s" (if state.Value then "active" else "")
                        OnClick(fun e -> setState (fun state props -> { state with Value = not state.Value })) ] [
                        Icons.comment
                    ]
                    br []
                    state.Value
                    &? div [ Class "event-message" ] [
                        AutoSync.TextArea.render
                            { Placeholder = Some "Beskjed til trenerne"
                              Url = sprintf "/api/events/%O/signup/message" event.Id
                              Value =
                                userAttendance
                                |> Option.map (fun ea -> ea.Message)
                                |> Option.defaultValue "" }
                       ]
                ])
        ]
    ]
