namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open System
open System.Linq


 type NotificationsModel = {
        UnansweredEventIds: Guid list
    } with 
        member m.UnansweredEvents = m.UnansweredEventIds.Length

[<AutoOpen>]
module Notifications =

    let key clubId =      
        sprintf "notifications-%O" clubId   


    let notifications (ctx: HttpContext) club (user: Users.User) =
        let (ClubId clubId) = club.Id
        let db = ctx.Database
        let now = DateTime.Now
        let inFourDays = now.AddDays(4.0)
          
        let getEventsAndAttendances () =
                
            let events = query  
                            { 
                                for e in db.Events do
                                where (e.ClubId = clubId 
                                            && e.DateTime < inFourDays 
                                            && e.DateTime > now 
                                            && not e.IsPublished) 
                                join eventTeam in db.EventTeams on (e.Id = eventTeam.EventId)                  
                                select (e.Id, eventTeam.TeamId)
                            } |> Seq.toList
                              |> List.distinct                       


            let eventIds =
                let eventIds = events |> List.map(fun (id, _) -> id) |> List.distinct
                query {
                    for e in eventIds do
                    select e
                }

            let attendances = 
                query {
                    for attendance in db.EventAttendances do
                    where (eventIds.Contains(attendance.EventId))
                    select (attendance.EventId, attendance.MemberId)
                }
                |> Seq.toList

            (events, attendances)
        
        
        let (events, attendances) =       
            Cache.get ctx (key clubId) getEventsAndAttendances


        let userEvents = events
                          |> List.filter (fun (_, teamId) -> user.TeamIds |> List.contains teamId)
                          |> List.map (fun (eventId, _) -> eventId)
                          |> List.distinct                       


        let userAttendances =
            attendances 
            |> List.filter (fun (_, memberId) -> memberId = user.Id)
            |> List.map (fun (eventId, _) -> eventId)

        let unansweredEventIds = 
            userEvents
            |> List.filter (fun (eventId) -> not (userAttendances |> List.contains eventId))
            |> List.distinct            
      

        let model = {
            UnansweredEventIds = unansweredEventIds
        }

        ul [_id "notification-button";_class "notification-button nav navbar-nav navbar-right navbar-topRight--item"] [
            (if model.UnansweredEvents > 0 then            
                li [_class "dropdown" ] [ 
                    button [_class "dropdown-toggle btn btn-warning"; attr "data-toggle" "dropdown" ] [
                        icon <| fa "bell-o"
                    ]
                    ul [_class "dropdown-menu" ] [
                        li [] [
                            a [_href <| sprintf "/intern#event-%O" (model.UnansweredEventIds |> Seq.head) ] [
                                Icons.signup
                                whitespace
                                whitespace
                                span [_class "hidden-xxs" ] [ 
                                    encodedText "Du har "
                                ]
                                encodedText <| sprintf "%i %s" model.UnansweredEvents (model.UnansweredEvents > 1 =? (" ubesvarte arrangementer", " ubesvart arrangement"))
                            ]
                        ]                    
                    ]
                ]
             else emptyText)
        ]

