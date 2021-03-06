namespace MyTeam.Attendance

open System
open System.Linq
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Events
open MyTeam.Common.Features.Members
open MyTeam.Models.Enums

module Queries =

    let getAttendance : GetAttendance = 
        fun db clubId year ->
            let (ClubId clubId) = clubId

            let trening = (int EventType.Trening)
            let kamp = (int EventType.Kamp)
            let years =
                 
                   query {
                            for e in db.EventAttendances do
                            where (                
                                    e.Event.ClubId = clubId &&
                                    (e.Event.Type =  trening || e.Event.Type = kamp)
                                  )
                            select e.Event.DateTime.Year
                            distinct
                   }
                   |> Seq.toList
                   |> List.sortDescending                                        
        
            let selectedYear = 
                match year with
                | Some y when isNumber y -> Year (int y)
                | Some y when y = "total" -> AllYears
                | Some y -> failwithf "Year må være en string, fikk %s" y
                | None -> Year (match years |> List.tryHead with
                                    | Some (y: int) -> y
                                    | None -> DateTime.Now.Year)

            let attendance = 
                let inOneHour = DateTime.Now.AddHours(1.0)
                    

                let attendances =
                    match selectedYear with
                    | Year y ->                     
                        query {
                            for a in db.EventAttendances do
                            where (a.Event.DateTime.Year = y)
                        }
                    | AllYears ->
                        query {
                            for a in db.EventAttendances do
                            select a                          
                        }
                    |> fun attendances ->
                        query {
                            for a in attendances do
                            where (
                                    a.Event.ClubId = clubId &&
                                    a.Event.DateTime < inOneHour &&
                                    (a.Event.Type = trening || a.Event.Type = kamp) &&
                                    a.Event.Voluntary <> true
                                )
                            select (a, a.Event.Type)
                        } 
                        |> Seq.toList


                let playerIds = (attendances |> List.map (fun (a,_) -> a.MemberId) |> List.distinct)
                let trener = (int PlayerStatus.Trener)
                let players = 
                    query {
                        for p in db.Members do
                        where (playerIds.Contains p.Id && p.Status <> trener)
                    } 
                    |> selectMembers
                    |> Seq.map 
                        (fun p ->
                            let attendances = attendances |> List.filter (fun (a, _) -> a.MemberId = p.Id)
                            (p,
                             {
                                Games = attendances 
                                        |> List.filter (fun (a, eventType) -> eventType = (int EventType.Kamp) && a.IsSelected) 
                                        |> List.length
                                Trainings = attendances 
                                            |> List.filter (fun (a, eventType) -> eventType = (int EventType.Trening) && a.DidAttend) 
                                            |> List.length
                                NoShows = attendances 
                                            |> List.filter (fun (a, eventType) -> eventType = (int EventType.Trening) && a.IsAttending = Nullable true && not a.DidAttend) 
                                            |> List.length
                                TrainingVictories = attendances 
                                            |> List.filter (fun (a, eventType) -> eventType = (int EventType.Trening) && a.WonTraining = true) 
                                            |> List.length
                            })
                        )    

                query {
                    for (p, a) in players do
                    where (a.Games + a.Trainings > 0)
                    sortByDescending a.Trainings
                    thenByDescending a.Games
                    thenByDescending a.TrainingVictories
                    thenByDescending a.NoShows
                } |> Seq.toList
        
            
            selectedYear, years, attendance
            
            
    let internal selectEvents (events: IQueryable<Models.Domain.Event>) = 
        query {
            for e in events do
                        select (e.Id, e.Location, e.DateTime)
        } |> Seq.map(fun (id, location, date) ->
                        {
                            Id = id
                            Date = date
                            Location = location                    
                        }
                    )

    let getPreviousTrainings: GetPreviousTrainings =
        fun db clubId ->
            let (ClubId clubId) = clubId
            let now = DateTime.Now
            let trening = int EventType.Trening
            query {
                for event in db.Events do
                where (event.ClubId = clubId && event.DateTime < now && event.Type = trening)
                sortByDescending event.DateTime
                take 15
            }
            |> selectEvents
            |> Seq.toList         
            
    let getTraining: GetTraining =
        fun db eventId ->
            db.Events.Where(fun e -> e.Id = eventId)
            |> selectEvents
            |> Seq.tryHead
                 
    let getPlayers: GetPlayers =
        fun db clubId eventId ->
            let (ClubId clubId) = clubId
            let players =
                let sluttet = int PlayerStatus.Sluttet
                let trener = int PlayerStatus.Trener
                query {
                    for p in db.Members do
                    where (p.ClubId = clubId && p.Status <> sluttet && p.Status <> trener)
                } 
                |> selectMembers
                |> Seq.toList

            let attendees = 
                query {
                    for a in db.EventAttendances do
                    where (a.EventId = eventId)
                    select (a.MemberId, a.DidAttend, a.IsAttending, a.WonTraining)
                } |> Seq.toList
                
            let players = 
                players
                |> List.map (fun p ->
                    let (playerDidAttend, playerWonTraining) = 
                        (attendees 
                         |> List.exists (fun (playerId, didAttend, _, _) -> p.Id = playerId && didAttend),
                         attendees 
                         |> List.exists (fun (playerId, _, _, wonTraining) -> p.Id = playerId && wonTraining))

                    p, playerDidAttend, playerWonTraining
                 )     
                |> List.sortBy (fun (p,_, _) -> p.Name)
          

            let playerIsAttending ((p, _, _): PlayerAttendance) =
                attendees |> List.exists (fun (id, _, isAttending, _) -> p.Id = id && (isAttending = Nullable true))

            let playerIsActive (p: Members.Member, _, _) =
                p.Status = Domain.PlayerStatus.Aktiv    

            let attendingPlayers = players |> List.filter (playerIsAttending)
            let othersActive = players |> List.filter playerIsActive |> List.except attendingPlayers
            let othersInactive = players |> List.except attendingPlayers |> List.except othersActive

            {
                Attending = attendingPlayers
                OthersActive = othersActive
                OthersInactive = othersInactive
            }

