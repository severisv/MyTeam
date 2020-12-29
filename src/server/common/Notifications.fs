module MyTeam.Notifications

open MyTeam
open Shared.Domain
open Shared.Domain.Members
open System
open System.Linq

type NotificationsModel =
    { UnansweredEventIds: Guid list }
    member m.UnansweredEvents = m.UnansweredEventIds.Length

let private eventsKey clubId =
    let (ClubId clubId) = clubId
    sprintf "notifications-%O" clubId

let private userAttendancesKey clubId userId =
    let (ClubId clubId) = clubId
    sprintf "notifications-%O-%O" clubId userId

let get (ctx: HttpContext) (club: Club) (user: User) =
    let (ClubId clubId) = club.Id
    let db = ctx.Database
    let now = DateTime.Now
    let inFourDays = now.AddDays(4.0)

    let getEvents () =
        db.Events.Where(fun e ->
          (e.ClubId = clubId
           && e.DateTime < inFourDays
           && e.DateTime > now
           && not e.IsPublished)).Select(fun e -> (e.Id, e.TeamId, e.EventTeams.Select(fun et -> et.TeamId)))
        |> Seq.toList
        |> List.map (function
            | (eventId, Value teamId, _) ->
                {| EventId = eventId
                   TeamIds = [ teamId ] |}
            | (eventId, _, teamIds) ->
                {| EventId = eventId
                   TeamIds = teamIds |> Seq.toList |}

            )
        |> List.distinct

    let events =
        Cache.get ctx (eventsKey club.Id) getEvents


    let getUserAttendances () =
        let eventIds =
            events
            |> List.map (fun e -> e.EventId)
            |> List.distinct

        db.EventAttendances.Where(fun ea ->
          ea.MemberId = user.Id
          && eventIds.Contains(ea.EventId)).Select(fun ea -> ea.EventId)
        |> Seq.toList
        |> List.distinct

    let attendances =
        Cache.get ctx (eventsKey club.Id) getUserAttendances


    let userEvents =
        events
        |> List.filter (fun e ->
            e.TeamIds
            |> List.exists (fun id -> (user.TeamIds |> List.contains id)))
        |> List.map (fun e -> e.EventId)
        |> List.distinct

    let unansweredEventIds =
        userEvents
        |> List.filter (fun eventId -> not (attendances |> List.contains eventId))
        |> List.distinct

    { UnansweredEventIds = unansweredEventIds }

let clearCacheForClub ctx clubId = Cache.clear ctx (eventsKey clubId)
let clearCacheForUser ctx clubId (user: User) = Cache.clear ctx (userAttendancesKey clubId user.Id)
