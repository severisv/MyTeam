module Server.Features.Fines.Api

open MyTeam
open Shared.Domain
open Shared.Features.Fines.Common

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


let listRemedyRates (club: Club) (db: Database) =
    let (ClubId clubId) = club.Id
    query {
        for rate in db.RemedyRates do
            where (rate.ClubId = clubId && rate.IsDeleted = false)
            select (rate.Id, rate.Name)
    }
    |> Seq.toList
    |> List.map(fun (id, name) ->
        { Id = id
          Name = name })
    |> OkResult