namespace MyTeam.Games

open MyTeam
open Shared.Domain
open MyTeam.Models.Domain
open System
open System.Linq
module Persistence =

    let selectPlayer: SelectPlayer = 
        fun db clubId eventId playerId model ->            
            let (ClubId clubId) = clubId
            query {
                for e in db.Events do
                where (e.ClubId = clubId && e.Id = eventId)
                select (e.Id)
            }
            |> Seq.toList
            |> List.tryHead
            |> function
                | None -> Unauthorized
                | Some e ->          
                    query {
                        for e in db.EventAttendances do
                            where (e.EventId = eventId && e.MemberId = playerId)
                            select e                       
                        }
                    |> Seq.tryHead
                    |> function
                        | Some a ->
                            a.IsSelected <- model.value                      
                        | None ->
                            let a = EventAttendance()                         
                            a.Id <- Guid.NewGuid()
                            a.EventId <- eventId
                            a.MemberId <- playerId                        
                            a.IsSelected <- model.value                        
                            db.EventAttendances.Add(a) |> ignore

                    db.SaveChanges() |> ignore                 
                    OkResult model


    let publishSquad = 
        fun (db: Database) clubId gameId ->
            let (ClubId clubId) = clubId

            db.Games.Where (fun e -> e.Id = gameId && e.ClubId = clubId)
            |> Seq.tryHead
            |> function
                | Some a ->
                    a.IsPublished <- true
                    db.SaveChanges() |> ignore
                    OkResult None

                | None ->
                    NotFound

            
                
