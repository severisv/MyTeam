namespace MyTeam.Events

open MyTeam
open MyTeam.Domain
open MyTeam.Models
open MyTeam.Models.Domain

module Persistence =
    let setDescription : SetDescription =
        fun db clubId memberId description -> 
            let (ClubId clubId) = clubId
            let event = db.Events
                        |> Seq.filter(fun p -> p.Id = memberId)
                        |> Seq.head

            if event.ClubId <> clubId then failwith "Prøver å redigere event fra annen klubb - ingen tilgang"
    
            event.Description <- description
            db.SaveChanges() |> ignore


  