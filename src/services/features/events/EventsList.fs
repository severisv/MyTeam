module Server.Features.Events.List

open System
open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Client.Events.List
open Shared
open System.Linq
open Strings

let internal view (period: Period) (club : Club) (user : User) (ctx : HttpContext) =
     let db = ctx.Database
     let (ClubId clubId) = club.Id
     let now = DateTime.Now
     let inTwoHours = now.AddHours -2.0
     let in14Days = now.AddDays 14.0
     
     let events =
         (match period with
         | Upcoming ->
              query {
                 for event in db.Events do             
                 where (event.ClubId = clubId &&
                        event.DateTime >= inTwoHours &&
                        (event.GameType = Nullable(int Models.Enums.GameType.Treningskamp) || event.DateTime < in14Days) 
                        )
             }
         | Previous ->
             query {
                 for event in db.Events do             
                 where (event.ClubId = clubId && event.DateTime < inTwoHours)
             })     
         |> fun events ->
             query {
                 for event in events do             
                 groupJoin ea in db.EventAttendances 
                   on (event.Id = ea.EventId) into attendances
                
                 for ea in attendances.DefaultIfEmpty() do
                 
                 select ({|Id = event.Id
                           Description = !!event.Description
                           DateTime = event.DateTime
                           Location = event.Location
                           Type = event.Type
                           GameType = event.GameType
                           Opponent = event.Opponent
                           TeamId = event.TeamId
                           SquadIsPublished = event.IsPublished
                         |}, {| FirstName = ea.Member.FirstName
                                LastName = ea.Member.LastName
                                UrlName = ea.Member.UrlName
                                Message = !!ea.SignupMessage
                                Id = Nullable ea.MemberId
                                IsAttending = ea.IsAttending
                                IsSelected = Nullable ea.IsSelected |}
                         )
             }
         |> Seq.toList
         |> List.groupBy (fun (e, _) -> e)
         
     let eventIds = query { for (e, _) in events do
                            select e.Id }
     let teamIds =
         query {
             for et in db.EventTeams do
                 where (eventIds.Contains(et.EventId))
                 select (et.EventId, et.TeamId)
         } |> Seq.toList
         
     let events =
         events
         |> List.map (fun (e, attendees) ->
             let eventType = e.Type |> Events.eventTypeFromInt
             let attendees = attendees
                            |> List.filter (fun (_, a) -> a.Id.HasValue)
                            |> List.sortBy (fun (_, a) -> a.FirstName)
             
             { Id = e.Id
               Description = e.Description
               DateTime = e.DateTime
               Location = e.Location
               Type = eventType
               TeamIds = if e.TeamId.HasValue then
                            [e.TeamId.Value]
                         else
                            teamIds |> List.filter(fun (eventId, _) -> eventId = e.Id)
                                    |> List.map (fun (_, teamId) -> teamId)
               Signups = attendees
                           |> List.map(fun (_, a) ->
                               { FirstName = a.FirstName
                                 LastName = a.LastName
                                 UrlName = a.UrlName
                                 Id = a.Id.Value
                                 Message = a.Message
                                 IsAttending = if a.IsAttending.HasValue then a.IsAttending.Value else false })
               Details =
                    (match eventType with
                     | Kamp -> 
                        let teamName = club.Teams
                                       |> List.tryFind (fun t -> Nullable t.Id = e.TeamId)
                                       |> Option.map(fun t -> t.ShortName)
                                       |> Option.defaultValue ""
                        Game { Team = teamName;
                               Opponent = e.Opponent
                               Type = Events.gameTypeFromInt e.GameType.Value
                               SquadIsPublished = e.SquadIsPublished
                               Squad = attendees
                                       |> List.filter (fun (_,a) -> a.IsSelected.Value)
                                       |> List.map(fun (_, a) ->
                                           { FirstName = a.FirstName
                                             LastName = a.LastName
                                             UrlName = a.UrlName
                                             Id = a.Id.Value
                                             Message = a.Message
                                             IsAttending = if a.IsAttending.HasValue then a.IsAttending.Value else false
                                           })}
                     | _ -> Training) })
         |> List.sortBy (fun e -> e.DateTime)

     
     [ Client.view
            containerId
            element
            { Events = events
              User = user
              Period = period } ]
     |> layout club (Some user) (fun o -> { o with Title = "Påmelding" }) ctx
     |> OkResult

let upcoming (club : Club) (user : User) (ctx : HttpContext) = view Upcoming club user ctx
let previous (club : Club) (user : User) (ctx : HttpContext) = view Previous club user ctx
