module MyTeam.Events.Api

open Client.Events
open MyTeam
open Shared
open Shared.Components.Input
open Shared.Domain
open Shared.Domain.Members
open Strings
open System
open System.Linq

type EventId = Guid

type Event = { Id: EventId; Description: string }

let setDescription clubId eventId (ctx: HttpContext) (model: StringPayload) =
    let db = ctx.Database
    let (ClubId clubId) = clubId

    query {
        for e in db.Events do
            where (e.Id = eventId)
            select e
    }
    |> Seq.head
    |> fun event ->

        if event.ClubId <> clubId then
            Unauthorized

        else
            event.Description <- model.Value
            db.SaveChanges() |> ignore
            OkResult None


let internal updateAttendance clubId (user: User) eventId (ctx: HttpContext) updateFn =

    let db = ctx.Database
    let (ClubId clubId) = clubId

    query {
        for e in db.Events do
            where (e.Id = eventId)
            select e.ClubId
    }
    |> Seq.head
    |> fun id ->

        if id <> clubId then
            Unauthorized

        else
            query {
                for ea in db.EventAttendances do
                    where (ea.MemberId = user.Id && ea.EventId = eventId)
                    select ea
            }
            |> Seq.tryHead
            |> function
                | Some ea -> ea
                | None ->
                    let ea = MyTeam.Models.Domain.EventAttendance(MemberId = user.Id, EventId = eventId)

                    db.EventAttendances.Add ea |> ignore
                    ea
            |> fun ea ->
                updateFn ea
                db.SaveChanges() |> ignore
                OkResult None


let signup clubId (user: User) eventId (ctx: HttpContext) (model: Signup) =
    updateAttendance clubId user eventId ctx (fun ea -> ea.IsAttending <- Nullable model.IsAttending)
    |> HttpResult.map (fun result ->
        Notifications.clearCacheForUser ctx clubId user
        result)

let signupMessage clubId (user: User) eventId (ctx: HttpContext) (model: StringPayload) =
    updateAttendance clubId user eventId ctx (fun ea -> ea.SignupMessage <- model.Value)



type DbEvent =
    { Id: Guid
      Description: string
      DateTime: DateTime
      Location: string
      Type: int
      GameType: Nullable<int>
      Opponent: string
      TeamId: Nullable<Guid>
      GamePlanIsPublished: Nullable<bool>
      SquadIsPublished: bool }


type DbM =
    { FirstName: string
      LastName: string
      UrlName: string
      Message: string
      Id: Nullable<Guid>
      IsAttending: Nullable<bool>
      IsSelected: Nullable<bool>
      DidAttend: Nullable<bool> }

let treningskamp = Nullable(int Models.Enums.GameType.Treningskamp)


let listEvents (club: Club) (user: User) period (db: Database) =
    let (ClubId clubId) = club.Id
    let now = DateTime.Now
    let inTwoHours = now.AddHours -2.0
    let in14Days = now.AddDays Event.allowedSignupDays

    let events =
        (match period with
         | Upcoming NearFuture ->
             query {
                 for event in db.Events do
                     where (
                         event.ClubId = clubId
                         && event.DateTime >= inTwoHours
                         && (event.GameType = treningskamp
                             || event.DateTime < in14Days)
                     )
             }
         | Upcoming Rest ->
             query {
                 for event in db.Events do
                     where (
                         event.ClubId = clubId
                         && event.DateTime >= inTwoHours
                         && (event.GameType <> treningskamp
                             && event.DateTime >= in14Days)
                     )
             }
         | Previous (Some year) ->
             query {
                 for event in db.Events do
                     where (
                         event.ClubId = clubId
                         && event.DateTime < inTwoHours
                         && event.DateTime.Year = year
                     )
             }
         | Previous None ->
             let year =
                 query {
                     for event in db.Events do
                         where (event.DateTime < inTwoHours)
                         sortByDescending event.DateTime.Year
                         take 1
                         select event.DateTime.Year
                 }
                 |> Seq.tryHead
                 |> Option.defaultValue now.Year

             query {
                 for event in db.Events do
                     where (
                         event.ClubId = clubId
                         && event.DateTime < inTwoHours
                         && event.DateTime.Year = year
                     )
             })
        |> fun events ->
            query {
                for event in events do
                    groupJoin ea in db.EventAttendances on (event.Id = ea.EventId) into group

                    for ea in group.DefaultIfEmpty() do
                        select (
                            { Id = event.Id
                              Description = event.Description
                              DateTime = event.DateTime
                              Location = event.Location
                              Type = event.Type
                              GameType = event.GameType
                              Opponent = event.Opponent
                              TeamId = event.TeamId
                              GamePlanIsPublished = event.GamePlanIsPublished
                              SquadIsPublished = event.IsPublished },
                            { FirstName = ea.Member.FirstName
                              LastName = ea.Member.LastName
                              UrlName = ea.Member.UrlName
                              Message = ea.SignupMessage
                              Id = Nullable ea.MemberId
                              IsAttending = ea.IsAttending
                              IsSelected = Nullable ea.IsSelected
                              DidAttend = Nullable ea.DidAttend }
                        )
            }
        |> Seq.toList
        |> List.groupBy (fun (e, _) -> e)

    let eventIds = events |> List.map (fun (e, _) -> e.Id)

    let teamIds =
        query {
            for et in db.EventTeams do
                where (eventIds.Contains(et.EventId))
                select (et.EventId, et.TeamId)
        }
        |> Seq.toList

    events
    |> List.map (fun (e, attendees) ->
        let eventType = e.Type |> Events.eventTypeFromInt

        let attendees =
            attendees
            |> List.filter (fun (_, a) -> a.Id.HasValue)
            |> List.sortBy (fun (_, a) -> a.FirstName)

        { Id = e.Id
          Description = !!e.Description
          DateTime = e.DateTime
          Location = e.Location
          Type = eventType
          TeamIds =
            if e.TeamId.HasValue then
                [ e.TeamId.Value ]
            else
                teamIds
                |> List.filter (fun (eventId, _) -> eventId = e.Id)
                |> List.map (fun (_, teamId) -> teamId)
          Signups =
            attendees
            |> List.map (fun (_, a) ->
                { FirstName = a.FirstName
                  LastName = a.LastName
                  UrlName = a.UrlName
                  Id = a.Id.Value
                  Message = !!a.Message
                  IsAttending = a.IsAttending |> fromNullable
                  DidAttend =
                    if a.DidAttend.HasValue then
                        a.DidAttend.Value
                    else
                        false })
          Details =
            (match eventType with
             | Kamp ->
                 let teamName =
                     club.Teams
                     |> List.tryFind (fun t -> Nullable t.Id = e.TeamId)
                     |> Option.map (fun t -> t.ShortName)
                     |> Option.defaultValue ""

                 Game
                     { Team = teamName
                       Opponent = e.Opponent
                       Type = Events.gameTypeFromInt e.GameType.Value
                       SquadIsPublished = e.SquadIsPublished
                       GamePlanIsPublished =
                         if e.GamePlanIsPublished.HasValue then
                             e.GamePlanIsPublished.Value
                         else
                             false
                       Squad =
                         attendees
                         |> List.filter (fun (_, a) -> a.IsSelected.Value)
                         |> List.map (fun (_, a) ->
                             { FirstName = a.FirstName
                               LastName = a.LastName
                               UrlName = a.UrlName
                               Id = a.Id.Value
                               Message = !!a.Message
                               IsAttending = a.IsAttending |> fromNullable
                               DidAttend =
                                 if a.DidAttend.HasValue then
                                     a.DidAttend.Value
                                 else
                                     false }) }
             | _ -> Training) })
    |> List.filter (fun e ->
        user.PlaysForTeam e.TeamIds
        || match e.Details with
           | _ when user.IsInRole [ Admin; Trener ] -> true
           | Game game -> game.SquadIsPublished
           | Training -> false)
    |> fun events ->
        match period with
        | Upcoming _ -> events |> List.sortBy (fun e -> e.DateTime)
        | Previous _ ->
            events
            |> List.sortByDescending (fun e -> e.DateTime)
    |> OkResult
