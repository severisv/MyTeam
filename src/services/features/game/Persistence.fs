namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Models.Domain
open System

module Persistence =

    let selectPlayer: SelectPlayer = 
        fun db clubId eventId playerId model ->

            let (ClubId clubId) = clubId

            query {
                for e in db.Events do
                where (e.ClubId = clubId && e.Id = eventId)
                select (e.Id)
            }
            |> Seq.tryHead
            |> function
                | None -> Unauthorized
                | Some e ->
                    db.EventAttendances 
                    |> Seq.tryFind (fun e -> e.EventId = eventId && e.MemberId = playerId)           
                    |> function
                        | Some a ->
                            a.IsSelected <- model.value
                            db.EventAttendances.Attach(a) |> ignore
                        | None ->
                            let a = EventAttendance()                         
                            a.Id <- Guid.NewGuid()
                            a.EventId <- eventId
                            a.MemberId <- playerId                        
                            a.IsSelected <- model.value                        
                            db.EventAttendances.Add(a) |> ignore

                    db.SaveChanges() |> ignore
                    OkResult model



    let publishSquad: PublishSquad = 
        fun db clubId gameId ->
            let (ClubId clubId) = clubId

            db.Games 
            |> Seq.tryFind (fun e -> e.Id = gameId && e.ClubId = clubId)    
            |> function
                | Some a ->
                    a.IsPublished <- true
                    db.SaveChanges() |> ignore
                    OkResult ()

                | None ->
                    NotFound

            
                
