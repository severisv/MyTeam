module Server.Features.Fines.Api

open MyTeam
open Shared.Domain

let delete (club : Club) fineId (db : Database) =
    let (ClubId clubId) = club.Id
    query {
        for fine in db.Fines do
            where (fine.Id = fineId && fine.Rate.ClubId = clubId)
            select fine
    }    
    |> Seq.tryHead
    |> function 
    | None -> NotFound
    | Some fine -> 
        db.Fines.Remove(fine) |> ignore
        db.SaveChanges() |> ignore
        OkResult()
