module Client.Events.SignupButtons

open Client.Components
open Client.Events
open Client.Components.Textarea
open Shared.Util.ReactHelpers
open Thoth.Json
open Fable.React
open Fable.React.Props
open Shared.Components
open Send
open Shared.Components.Base
open System

type Props = {
    HandleSignup: bool -> string -> unit
    EventId: Guid
    UserAttendance: Attendee option
}

type BoolState = { Value: bool }

let element props =

    let userAttendance = props.UserAttendance
    let userIsAttending =
                userAttendance |> Option.map(fun ea -> ea.IsAttending)
                               |> Option.defaultValue false

    let event = {| Id = props.EventId |}
    
    div [Class "event-signupButtons" ]
        [ sendElement (fun o ->
                        { o with
                           IsSent = Some userIsAttending
                           SentClass = None
                           SentIndicator = None
                           Spinner = None
                           OnSubmit = Some <| props.HandleSignup true
                           SendElement = btn, [Lg], [str "Stiller"] 
                           SentElement = btn, [Success;Lg], [str "Stiller"]
                           Endpoint = Put(sprintf "/api/events/%O/signup" event.Id,
                                          Some <| fun () ->  Encode.Auto.toString(0, { IsAttending = true })) })
          sendElement (fun o ->
                        { o with
                           IsSent = Some <| (not userIsAttending && userAttendance.IsSome)
                           SentClass = None
                           Spinner = None
                           SentIndicator = None
                           OnSubmit = Some <| props.HandleSignup false
                           SendElement = btn, [Lg], [str "Kan ikke"] 
                           SentElement = btn, [Danger;Lg], [str "Kan ikke"]
                           Endpoint = Put(sprintf "/api/events/%O/signup" event.Id,
                                          Some <| fun () ->  Encode.Auto.toString(0, { IsAttending = false })) })
          
          komponent<unit, BoolState>
              ()
              { Value = false}
              None
              (fun (props, state, setState) ->
                  fr [] [
                      a [ Title "Beskjed til trenerne"
                          Class <| sprintf "event-addMessage %s" (if state.Value then "active" else "")
                          OnClick (fun e -> setState(fun state props -> { state with Value = not state.Value }))
                      ] [Icons.comment]
                      br []
                      state.Value &?
                          div [Class "event-message"] [
                              Textarea.render { Placeholder = Some "Beskjed til trenerne"
                                                Url = sprintf "/api/events/%O/signup/message" event.Id
                                                Value = userAttendance |> Option.map(fun ea -> ea.Message) |> Option.defaultValue ""}
                  ]]
            )
        ]