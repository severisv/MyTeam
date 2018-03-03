namespace MyTeam.Attendance

open System.Linq
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Domain.Events
open MyTeam.Models.Enums
open System

module Queries =

    let getAttendance : GetAttendance = 
        fun db clubId year ->
            let (ClubId clubId) = clubId

            let years = 
                   query {
                            for e in db.EventAttendances do
                            where (                
                                    e.Event.ClubId = clubId &&
                                    (e.Event.Type = (int EventType.Trening) || e.Event.Type = (int EventType.Kamp))
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
                let attendanceQuery =
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

                let attendances = 
                    query {
                        for a in attendanceQuery do
                        where (
                                a.Event.ClubId = clubId &&
                                a.Event.DateTime < inOneHour &&
                                (a.Event.Type = (int EventType.Trening) || a.Event.Type = (int EventType.Kamp)) &&
                                a.Event.Voluntary <> true
                            )
                        select (a, a.Event.Type)
                    } 
                    |> Seq.toList


                let playerIds = (attendances |> Seq.map (fun (a,_) -> a.MemberId) |> Seq.distinct)
                let players = 
                    query {
                        for p in db.Members do
                        where (playerIds.Contains(p.Id) && p.Status <> (int PlayerStatus.Trener))
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
                            })
                        )    

                query {
                    for (p, a) in players do
                    where (a.Games + a.Trainings > 0)
                    sortByDescending a.Trainings
                    thenByDescending a.Games
                    thenByDescending a.NoShows
                } |> Seq.toList
        
            
            selectedYear, years, attendance
            


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
                select 
                    ({
                        Id = event.Id
                        Date = event.DateTime
                        Location = event.Location                    
                    })
            }
            |> Seq.toList         


    let getTraining: GetTraining =
        fun db eventId ->
            query {
                for event in db.Events do
                where (event.Id = eventId)
                select 
                    ({
                        Id = eventId
                        Date = event.DateTime
                        Location = event.Location                    
                    })
            }
            |> Seq.head
                 
    let getPlayers: GetPlayers =
        fun db clubId eventId ->
            let (ClubId clubId) = clubId
            let players =
                let sluttet = int PlayerStatus.Sluttet
                let trener = int PlayerStatus.Trener
                query {
                    for p in db.Players do
                    where (p.ClubId = clubId && p.Status <> sluttet && p.Status <> trener)
                    select (p.Id, 
                            p.FirstName, 
                            p.MiddleName, 
                            p.LastName, 
                            p.FacebookId, 
                            p.ImageFull, 
                            p.Status, 
                            p.UrlName)
                } 
                |> Seq.toList

            let attendees = 
                query {
                    for a in db.EventAttendances do
                    where (a.EventId = eventId)
                    select (a.MemberId, a.DidAttend, a.IsAttending)
                } |> Seq.toList
                
            let players = 
                players
                |> List.map (fun (id, firstName, middleName, lastName, facebookId, image, status, urlName) ->
                    let playerDidAttend = 
                        attendees 
                        |> List.exists (fun (playerId, didAttend, _) -> id = playerId && didAttend)

               

                    {
                        Id = id
                        FacebookId = facebookId
                        FirstName = firstName
                        MiddleName = middleName
                        LastName = lastName
                        UrlName = urlName
                        Image = image
                        Status = enum<PlayerStatus> status
                    }, playerDidAttend

                 )     
                |> List.sortBy (fun (p,_) -> p.Name)
          

            let playerIsAttending ((p, _): PlayerAttendance) =
                attendees |> List.exists (fun (id, _, isAttending) -> p.Id = id && (isAttending = Nullable true))

            let playerIsActive (p, _) =
                p.Status = PlayerStatus.Aktiv    

            let attendingPlayers = players |> List.filter (playerIsAttending)
            let othersActive = players |> List.filter playerIsActive |> List.except attendingPlayers
            let othersInactive = players |> List.except attendingPlayers |> List.except othersActive

            {
                Attending = attendingPlayers
                OthersActive = othersActive
                OthersInactive = othersInactive
            }