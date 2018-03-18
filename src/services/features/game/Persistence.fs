namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Models.Domain
open System

module Persistence =

    let selectPlayer: SelectPlayer = 
        fun clubId eventId playerId isSelected db ->
            let (ClubId clubId) = clubId

            query {
                for e in db.Events do
                where (e.ClubId = clubId && e.Id = eventId)
                select (e.Id)
            }
            |> Seq.tryHead
            |> function
                | None -> Error AuthorizationError
                | Some e ->
                    db.EventAttendances 
                    |> Seq.tryFind (fun e -> e.EventId = eventId && e.MemberId = playerId)           
                    |> function
                        | Some a ->
                            a.IsSelected <- isSelected
                            db.EventAttendances.Attach(a) |> ignore
                        | None ->
                            let a = EventAttendance()                         
                            a.Id <- Guid.NewGuid()
                            a.EventId <- eventId
                            a.MemberId <- playerId                        
                            a.IsSelected <- isSelected                        
                            db.EventAttendances.Add(a) |> ignore

                    db.SaveChanges() |> ignore
                    Ok ()
