module Server.Features.Fines.Api

open MyTeam
open MyTeam.Models.Domain
open Shared.Domain
open Shared.Features.Fines.Add
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
            select (rate.Id, rate.Name, rate.Rate)
    }
    |> Seq.toList
    |> List.map(fun (id, name, amount) ->
        { Id = id
          Amount = amount
          Name = name })
    |> List.sortBy (fun r -> r.Name)
    |> OkResult
    
    
let add (club: Club) (ctx: HttpContext) (model: AddFine) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id
    query {
        for memb in db.Members do
        where (clubId = memb.ClubId && memb.Id = model.MemberId)
        select (memb.Id)
    }
    |> Seq.toList
    |> List.tryHead
    |> function
    | Some _ ->
         let fine = Fine(MemberId = model.MemberId,
                         RemedyRateId = model.RateId,
                         Amount = model.Amount,
                         Issued = System.DateTime.Now,
                         Comment = model.Comment,
                         RateName = model.RateName)
         db.Fines.Add(fine) |> ignore
         db.SaveChanges() |> ignore
         OkResult { model with Id = Some fine.Id }
    | None -> Unauthorized