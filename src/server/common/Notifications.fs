module MyTeam.Notifications

open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open System
open System.Linq

type NotificationsModel =
    { UnansweredEventIds : Guid list }
    member m.UnansweredEvents = m.UnansweredEventIds.Length

let private key clubId =
    let (ClubId clubId) = clubId
    sprintf "notifications-%O" clubId


let get (ctx : HttpContext) (club: Club) (user : User) =
    let (ClubId clubId) = club.Id
    let db = ctx.Database
    let now = DateTime.Now
    let inFourDays = now.AddDays(4.0)
    
    let getEventsAndAttendances() =
        let events =
            query { 
                for e in db.Events do
                    where 
                        (e.ClubId = clubId && e.DateTime < inFourDays && e.DateTime > now 
                         && not e.IsPublished)
                    join eventTeam in db.EventTeams on (e.Id = eventTeam.EventId)
                    select (e.Id, eventTeam.TeamId)
            }
            |> Seq.toList
            |> List.distinct
        
        let eventIds =
                events
                |> List.map (fun (id, _) -> id)
                |> List.distinct
          
        
        let attendances =
            query { 
                for attendance in db.EventAttendances do
                    where (eventIds.Contains(attendance.EventId))
                    select (attendance.EventId, attendance.MemberId)
            }
            |> Seq.toList
        
        (events, attendances)
    
    let (events, attendances) = Cache.get ctx (key club.Id) getEventsAndAttendances
    
    let userEvents =
        events
        |> List.filter (fun (_, teamId) -> user.TeamIds |> List.contains teamId)
        |> List.map (fun (eventId, _) -> eventId)
        |> List.distinct
    
    let userAttendances =
        attendances
        |> List.filter (fun (_, memberId) -> memberId = user.Id)
        |> List.map (fun (eventId, _) -> eventId)
    
    let unansweredEventIds =
        userEvents
        |> List.filter (fun eventId -> not (userAttendances |> List.contains eventId))
        |> List.distinct
    
    { UnansweredEventIds = unansweredEventIds }

let clearCache ctx clubId =
    Cache.clear ctx (key clubId)
    Cache.clear ctx ("old-" + (key clubId))
