module Client.Events.List

open Client.Components
open Client.Components.Textarea
open Shared.Util.ReactHelpers
open Thoth.Json
open Fable.React
open Fable.React.Props
open Shared
open Shared.Components.Layout
open Shared.Components.Tabs
open Shared.Components
open Send
open Shared.Components
open Shared.Components.Icons
open Shared.Components.Base
open Shared.Components.Nav
open Shared.Domain
open Shared.Domain.Members
open System

type Attendee = {
    Id: Guid
    FirstName: string
    LastName: string
    UrlName: string
    IsAttending: bool
    Message: string
}

type Game = {
    Team: string
    Opponent: string
    Type: GameType
    SquadIsPublished: bool
    Squad: Attendee list
}


type Details =
    | Game of Game
    | Training

type Event = {
    Id: Guid
    Type: EventType
    DateTime: DateTime
    Location: string
    Description: string
    Details: Details
    TeamIds: Guid list
    Signups: Attendee list
 }


type Model = {
    User: User
    Events: Event list
 }

type Signup = {
    IsAttending: bool
}

type State = {
    Signups: Map<Guid, Attendee list>
}

type BoolState = { Value: bool }

let containerId = "list-events"
let element props children =
        komponent<Model, State>
             props
             { Signups = props.Events
                            |> List.map(fun e -> (e.Id, e.Signups))
                            |> Map.ofList }
             None
             (fun (props, state, setState) ->
                let user = props.User
                fragment [] [
                    mtMain [] [
                        block [] [
                          div [ Class "row" ] [
                              div [ Class "col-sm-7 col-xs-12" ] [
                                  tabs []
                                       [{ Text = " Kommende"
                                          ShortText = " Kommende"
                                          Url = "/intern"
                                          Icon = Some(Icons.calendar "") }
                                        { Text = " Tidligere"
                                          ShortText = " Tidligere"
                                          Url = "/intern/arrangementer/tidligere"
                                          Icon = Some(Icons.previous "") } ]
                                       ((=) "/intern")
                              ]
                          ]
                          fr [] 
                              (props.Events
                              |> List.map (fun event ->
                                   
                                    let signups = state.Signups.[event.Id]
                                    let attending = signups |> List.filter (fun ea -> ea.IsAttending)
                                    let notAttending = signups |> List.filter (fun ea -> not ea.IsAttending)
                                    let userAttendance = signups |> List.tryFind (fun ea -> ea.Id = user.Id)
                                    let userIsAttending =
                                            userAttendance |> Option.map(fun ea -> ea.IsAttending)
                                                           |> Option.defaultValue false
                                                                       
                                    let handleSignup isAttending _ =
                                        setState (fun state props ->
                                            let ea = signups
                                                     |> List.tryFind(fun ea -> ea.Id = user.Id)
                                                     |> function
                                                     | Some ea -> { ea with IsAttending = isAttending }
                                                     | None -> { Id = user.Id
                                                                 FirstName = user.FirstName
                                                                 LastName = user.LastName
                                                                 UrlName = user.UrlName
                                                                 Message = ""
                                                                 IsAttending = isAttending  }
                                            
                                            let modifiedAttendees = signups
                                                                    |> List.filter (fun a -> a.Id <> user.Id)
                                                                    |> List.append [ea]
                                                                    |> List.sortBy (fun a -> a.FirstName)
                                            { state with
                                                Signups = state.Signups
                                                            |> Map.map (fun eventId attendees ->
                                                                            if eventId = event.Id then
                                                                                modifiedAttendees
                                                                            else attendees
                                                                        ) } )
                                       
                                    fragment [] [   
                                      hr []
                                      div [ Class "show-upcoming-event" ]
                                            [ div [ Class "event-editButtons" ] [
                                                  props.User.IsInRole [Role.Trener;Role.Admin] &?
                                                    Links.editAnchor [ Href <| sprintf "/intern/arrangement/endre/%O" event.Id ]
                                              ]
                                              div [ Id <| sprintf "event-%O" event.Id
                                                    Class "hashlink-anchor" ] []
                                              div [ Class "show-event-container" ] [
                                                div [ Class "event-col-1 event-icon" ] [
                                                    div [] [
                                                        a [ Href <| sprintf "#event-%O" event.Id ] [
                                                            Icons.eventIcon event.Type IconSize.Normal
                                                          ]
                                                        ]
                                                ] 
                                                div [ Class "event-col-2" ] [
                                                    p [] [
                                                        Icons.calendar ""
                                                        whitespace
                                                        str <| Date.format event.DateTime ]
                                                    p [] [
                                                      Icons.time ""
                                                      whitespace
                                                      str <| Date.formatTime event.DateTime ]
                                                    p [] [
                                                        Icons.mapMarker ""
                                                        whitespace
                                                        str event.Location ]
                                                ]
                                                div [ Class "event-col-3 " ] [
                                                         (match event.Details with
                                                          | Game game ->
                                                                 fr [] [
                                                                     h3 [Class "no-margin"][
                                                                         str <| sprintf "%s " game.Team
                                                                         span [Class "event-opponentDivider" ][ str "vs" ]
                                                                         str <| sprintf " %s" game.Opponent
                                                                     ]
                                                                     p [] [
                                                                         Icons.gameType game.Type
                                                                         whitespace
                                                                         str <| string game.Type
                                                                     ]
                                                                     game.SquadIsPublished &?
                                                                        str event.Description
                                                                 ]
                                                          | _ -> str event.Description)
                                                    ] ]
                                          ]
                                      div
                                        [ Class <| sprintf "event-signup--%s" (string event.Type |> Strings.toLower) ]
                                        [ div []
                                            [
                                              div [Class "clearfix"] []
                                              (match event.Details with
                                               | Game game when game.SquadIsPublished -> empty
                                               | _ ->
                                                  div [Class "event-signupButtons" ]
                                                    [ sendElement (fun o ->
                                                                    { o with
                                                                       IsSent = Some userIsAttending
                                                                       SentClass = None
                                                                       SentIndicator = None
                                                                       Spinner = None
                                                                       OnSubmit = Some <| handleSignup true
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
                                                                       OnSubmit = Some <| handleSignup false
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
                                                                      OnClick (fun _ -> setState(fun state props -> { state with Value = not state.Value }))
                                                                  ] [Icons.comment]
                                                                  br []
                                                                  div [Class <| sprintf "event-message %s" (if state.Value then "" else "hidden")] [
                                                                      Textarea.render { Placeholder = Some "Beskjed til trenerne"
                                                                                        Url = sprintf "/api/events/%O/signup/message" event.Id
                                                                                        Value = userAttendance |> Option.map(fun ea -> ea.Message) |> Option.defaultValue ""}
                                                              ]]
                                                        )
                                                    ])
                                              br [ ]
                                              div [ Class "event-signup-listplayers" ] [
                                                (match (event.Details) with
                                                | Training ->
                                                    
                                                    let attendeeLink (ea: Attendee) =
                                                        li [ ] [
                                                            a [ Class "attendee underline"
                                                                Href <| sprintf "/spillere/vis/%s" ea.UrlName ] [
                                                                Icons.user ""
                                                                whitespace
                                                                span [ ] [ str ea.FirstName ]
                                                                whitespace
                                                                span [ Class "hidden-xs" ] [ str ea.LastName ]
                                                                span [ Class "hidden-sm hidden-md hidden-lg" ] [ str <| string ea.LastName.[0]]
                                                            ]
                                                            (user.IsInRole [Role.Trener] && Strings.hasValue ea.Message) &?
                                                                tooltip ea.Message [] [Icons.comment]
                                                            ]
                                                    
                                                    Collapsible.collapsible Collapsed [
                                                          span [Class "flex-2" ]
                                                            [str <| sprintf "Kommer (%i)" attending.Length]
                                                          span [ Class "flex-2" ]
                                                            [str <| sprintf "Kan ikke (%i)" notAttending.Length]
                                                          span [Class "flex-1 pull-right hidden-xs" ] [
                                                              whitespace
                                                          ]
                                                        ] [
                                                          hr [Class "sm"]
                                                          div [Class "flex event-attendees "]
                                                            [ ul [ Class "list-unstyled flex-2" ]
                                                                    (attending |> List.map attendeeLink)
                                                              ul [ Class "list-unstyled flex-2" ]
                                                                    (notAttending |> List.map attendeeLink)
                                                              div [ Class "flex-1 hidden-xs" ][ whitespace ] ] 
                                                           ]
                                                    | Game game when game.SquadIsPublished ->
                                                        Collapsible.collapsible Open [
                                                                str <| sprintf "Tropp (%O)" game.Squad.Length
                                                            ]
                                                            [
                                                                fr [] [
                                                                    hr [Class "sm"]
                                                                    div [Class "event-attendees"] [
                                                                        ul [Class "list-unstyled flex-2"]
                                                                            (game.Squad
                                                                             |> List.map (fun ea -> 
                                                                                    li [] [
                                                                                        a [Class <| sprintf "attendee underline %s"
                                                                                                        (if props.User.Id = ea.Id then "userPlayer" else "")
                                                                                           Href <| sprintf "/spillere/vis/%s" ea.UrlName] [
                                                                                            Icons.player ""
                                                                                            whitespace
                                                                                            str ea.FirstName
                                                                                            whitespace
                                                                                            str ea.LastName
                                                                                        ]
                                                                                        (user.IsInRole [Role.Trener] && Strings.hasValue ea.Message)
                                                                                            &? tooltip ea.Message [] [Icons.comment]
                                                                                    ]))
                                                                        ] ]
                                                            ]
                                                    | _ -> div [] []
                                                    )   
                                             ] ] ]
                                    ]))
                        ]
                    ]                    
                    sidebar [] [
                        user.IsInRole [Admin;Trener] &?
                            block [] [
                                navListBase [ Header <| str "Admin" ] [
                                            a [Href "/intern/arrangement/ny"] [Icons.add "";whitespace;str "Opprett trening"]
                                           ]
                            ]
                    ]
                ]


        )

hydrate containerId Decode.Auto.fromString<Model> element
