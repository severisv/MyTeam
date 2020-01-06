module Client.Events.List

open Client.Components
open Shared.Util.ReactHelpers
open Thoth.Json
open Fable.React
open Fable.React.Props
open Shared
open Shared.Components.Layout
open Shared.Components.Tabs
open Shared.Components
open Shared.Components.Icons
open Shared.Components.Base
open Shared.Components.Nav
open Shared.Domain.Members
open System
open Client.Events
open Client.Util

type Model = {
    User: User
    Events: Event list
    Period: Period
    Years: int list
}

type FetchMoreState =
    | Unfetched
    | Success
    | Fetching
    | Error of string

type State = {
    Signups: Map<Guid, Attendee list>
    Events: Event list
    FetchMoreState: FetchMoreState
}

let createUrl =
    function
    | Upcoming _ -> "/intern"
    | Previous (Some year) -> sprintf "/intern/arrangementer/tidligere/%i" year
    | Previous None -> "/intern/arrangementer/tidligere"
    

let attendeeLink (user: User) (ea: Attendee) =
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
    
let loadMoreEvents setState _ =
    setState (fun state props ->
                        { state with FetchMoreState = Fetching })   
    Http.get "/api/events/upcoming" Decode.Auto.fromString<Event list>
                  { OnSuccess = fun result -> setState (fun state props ->
                      
                        { state with Events = state.Events @ result
                                     FetchMoreState = Success })                                                                                   
                    OnError = fun _ -> setState (fun state props ->
                        { state with FetchMoreState = Error "Noe gikk galt ved lasting
                          av arrangementer." }) }    

let containerId = "list-events"
let element props children =
        komponent<Model, State>
             props
             { Events = props.Events
               FetchMoreState = Unfetched
               Signups = props.Events
                            |> List.map(fun e -> (e.Id, e.Signups))
                            |> Map.ofList }
             None
             (fun (_, state, setState) ->
                let user = props.User
                
                let isSelected (url: string) =
                    match props.Period with
                    | Previous None when url.Contains (props.Years
                                                       |> List.tryHead
                                                       |> Option.map string
                                                       |> Option.defaultValue "") -> true
                    | _ -> url = (createUrl props.Period)

                fragment [] [
                    mtMain [] [
                        block [] [
                          div [ Class "row" ] [
                              div [ Class "col-sm-7 col-xs-12" ] [
                                  tabs []
                                       [{ Text = " Kommende"
                                          ShortText = " Kommende"
                                          Url = createUrl (Upcoming NearFuture)
                                          Icon = Some(Icons.calendar "") }
                                        { Text = " Tidligere"
                                          ShortText = " Tidligere"
                                          Url = createUrl (Previous None)
                                          Icon = Some(Icons.previous "") } ]
                                       (fun url -> match props.Period with | Previous _ -> url.Contains "tidligere" | Upcoming _ -> url.Contains "tidligere" |> not)
                              ]
                              navListMobile
                                 { Items = props.Years |> List.map (fun year  -> { Text = string year; Url = createUrl (Previous <| Some year)})  
                                   Footer = None                                                               
                                   IsSelected = isSelected }
                          ]
                          fr [] 
                              (state.Events
                              |> List.map (fun event ->
                                   
                                    let signups = state.Signups.TryFind(event.Id) |> Option.defaultValue []
                                    let attending = signups |> List.filter (fun ea -> ea.IsAttending)
                                    let notAttending = signups |> List.filter (fun ea -> not ea.IsAttending)
                                    let didAttend = signups |> List.filter (fun ea -> not ea.DidAttend)
                                    let userAttendance = signups |> List.tryFind (fun ea -> ea.Id = user.Id)                                 
                                                                       
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
                                                                 IsAttending = isAttending
                                                                 DidAttend = false }
                                            
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
                                            [ div [ Class "event-editButtons" ] 
                                                  ([ props.User.IsInRole [Trener; Admin] &?
                                                        Links.editAnchor [Href <| sprintf "/intern/arrangement/endre/%O" event.Id ] ] @
                                                   (match (event.Details, props.Period) with
                                                    | (Game game, _) when user.IsInRole [Trener] -> [
                                                        a [Class "edit-link pull-right"; Href <| sprintf "/kamper/%O/bytteplan" event.Id] [Icons.gamePlan]
                                                        a [Class "edit-link pull-right"; Href <| sprintf "/kamper/%O/laguttak"  event.Id] [Icons.teamSelection]
                                                        ]
                                                    | (Game game, _) when game.GamePlanIsPublished -> [
                                                        a [Class "edit-link pull-right"; Href <| sprintf "/kamper/%O/bytteplan" event.Id] [Icons.gamePlan]
                                                        ]    
                                                    | (Game game, _) -> []
                                                    | (Training, Previous _) when user.IsInRole [Admin; Trener; Oppmøte] -> [
                                                        a [Class "edit-link pull-right"; Href <| sprintf "/intern/oppmote/registrer/%O" event.Id] [Icons.attendance "Registrer oppmøte"]
                                                        ]
                                                    | (Training, _) -> []
                                                    )
                                                 )
                                       
                                                     
                                              div [ Id <| sprintf "event-%O" event.Id
                                                    Class "hashlink-anchor" ] []
                                              div [ Class "show-event-container" ] [
                                                div [ Class "event-col-1 event-icon" ] [
                                                    div [] [
                                                        a [Href <| sprintf "#event-%O" event.Id ] [
                                                           Icons.eventIcon event.Type IconSize.Normal]
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
                                                      //str <| Date.formatTime event.DateTime
                                                      str event.TimeString]
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
                                                                     str event.Description
                                                                 ]
                                                          | _ -> str event.Description)
                                                    ] ]
                                          ]
                                      div
                                        [ Class <| sprintf "event-signup--%s" (string event.Type |> Strings.toLower) ]
                                          (
                                           [div [Class "clearfix"] []] @
                                           match (event.Details, props.Period) with
                                               | (_ , Upcoming _) when not <| Event.signupHasOpened event -> []
                                               | (Game game, Upcoming _) when not game.SquadIsPublished ->
                                                  [
                                                    user.PlaysForTeam event.TeamIds &?
                                                        SignupButtons.element { Event = event
                                                                                UserAttendance = userAttendance
                                                                                HandleSignup = handleSignup }
                                                    
                                                  ]                                          
                                               | (Game game, _)  -> [
                                                   div [Class "event-signup-listplayers"; Style [MarginTop "0.9em"]] [
                                                        Collapsible.collapsible (props.Period |> function | Upcoming _ -> Open | Previous _ -> Collapsed) [
                                                                str <| sprintf "Tropp (%O)" game.Squad.Length
                                                            ]
                                                            [
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
                                                                    ] 
                                                            ]
                                                        ]
                                                   ]   
 
                                               | (Training, Upcoming _) ->
                                                  [
                                                    SignupButtons.element { Event = event
                                                                            UserAttendance = userAttendance
                                                                            HandleSignup = handleSignup }
                                                    br [ ]
                                                    div [ Class "event-signup-listplayers" ] [
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
                                                                    (attending |> List.map (attendeeLink user))
                                                              ul [ Class "list-unstyled flex-2" ]
                                                                    (notAttending |> List.map (attendeeLink user))
                                                              div [ Class "flex-1 hidden-xs" ][ whitespace ] ] 
                                                           ]
                                                        ]
                                                  ]
                                               | (Training, Previous _) ->
                                                  [
                                                     div [ Class "event-signup-listplayers" ] [
                                                        Collapsible.collapsible Collapsed [
                                                          span [Class "flex-2" ]
                                                            [str <| sprintf "Oppmøte (%i)" didAttend.Length] 
                                                        ] [
                                                          div [Class "event-attendees "]
                                                            [ ul [ Class "list-unstyled flex-2" ]
                                                            (didAttend |> List.map (attendeeLink user))
                                                             ]
                                                           ]
                                                        ]                                                  
                                                  ]
                                          )                                        
                                        ]
                                    ))
                          
                          props.Period = Upcoming NearFuture &?
                            div [Class "event-showAll"] [
                              (match state.FetchMoreState with
                               | Unfetched ->     
                                    a [OnClick <| loadMoreEvents setState][
                                        Icons.addCircle ""
                                        str " Vis alle"
                                    ]
                               | Fetching -> Icons.spinner
                               | Error e -> fr [] [
                                   div [Class "red"] [Icons.warning; whitespace; str e]
                                   a [OnClick <| loadMoreEvents setState][
                                        str "Prøv på nytt"
                                    ]
                                   ]
                               | Success -> empty)
                              ]
                        ]
                      
                    ]                    
                    sidebar [] [                        
                        props.Years.Length > 0 &?                                 
                        block [] [
                            navList
                                 {
                                   Header = "Sesonger"
                                   Items = props.Years |> List.map (fun year  -> { Text = [str <| string year]; Url = createUrl (Previous <| Some year)})  
                                   Footer = None                                                               
                                   IsSelected = isSelected}
                            ]   
                        user.IsInRole [Admin;Trener] &?
                            block [] [
                                navListBase [ Header <| str "Admin" ] [
                                            a [Href "/intern/arrangement/ny"] [Icons.add "";whitespace;str "Legg til trening"]
                                            a [Href "/intern/arrangement/ny?type=kamp"] [Icons.add "";whitespace;str "Legg til kamp"]
                                           ]
                            ]
                    ]
                ]


        )

hydrate containerId Decode.Auto.fromString<Model> element
