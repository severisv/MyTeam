namespace MyTeam.Attendance

open MyTeam
open MyTeam.Domain
open MyTeam.Models.Domain
open System
open MyTeam.Attendance

module Persistence =

    let confirmAttendance: ConfirmAttendance = 
        fun clubId eventId playerId didAttend db ->
            let (ClubId clubId) = clubId

            query {
                for e in db.Events do
                where (e.ClubId = clubId && e.Id = eventId)
                select (e.Id)
            }
            |> Seq.tryHead
            |> function
                | None -> Error AuthorizationError
                | Some eventId ->
                    db.EventAttendances 
                    |> Seq.tryFind (fun e -> e.EventId = eventId && e.MemberId = playerId)
                    |> function 
                        | Some attendance ->
                            attendance.DidAttend <- didAttend
                            db.EventAttendances.Attach(attendance) |> ignore
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