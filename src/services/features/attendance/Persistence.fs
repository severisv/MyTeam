namespace MyTeam.Attendance

open MyTeam
open MyTeam.Domain
open MyTeam.Models.Domain
open System
open MyTeam.Attendance

module Persistence =

    let confirmAttendance: ConfirmAttendance = 
        fun db clubId eventId playerId didAttend ->
            let (ClubId clubId) = clubId

            let event = 
                query {
                    for e in db.Events do
                    where (e.ClubId = clubId && e.Id = eventId)
                    select (e.Id)
                }
                |> Seq.tryHead

            match event with
            | None -> Error AuthorizationError
            | Some e ->
                let attendance =
                    db.EventAttendances 
                    |> Seq.tryFind (fun e -> e.EventId = eventId && e.MemberId = playerId)            
   
                match attendance with
                | Some a ->
                    a.DidAttend <- didAttend
                    db.EventAttendances.Attach(a) |> ignore
                | None ->
                    let a = EventAttendance()                         
                    a.Id <- Guid.NewGuid()
                    a.EventId <- eventId
                    a.DidAttend <- didAttend
                    a.IsAttending <- Nullable false
                    a.MemberId <- playerId                        
                    db.EventAttendances.Add(a) |> ignore

                db.SaveChanges() |> ignore
                Ok ()