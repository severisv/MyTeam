module Server.Features.Fines.Api

open MyTeam
open MyTeam.Models.Domain
open Shared
open Shared.Domain
open Shared.Features.Fines.List
open Shared.Features.Fines.Common
open Strings

let listRemedyRates (club: Club) (db: Database) =
    let (ClubId clubId) = club.Id
    query {
        for rate in db.RemedyRates do
            where (rate.ClubId = clubId && rate.IsDeleted = false)
            select (rate.Id, rate.Name, rate.Rate, rate.Description)
    }
    |> Seq.toList
    |> List.map(fun (id, name, amount, description) ->
        { Id = id
          Amount = amount
          Name = !!name
          Description = !!description })
    |> List.sortBy (fun r -> r.Name)
    
    
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

let addRemedyRate (club: Club) (ctx: HttpContext) (model: RemedyRate) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id
  
    let rate = RemedyRate(Name = model.Name,
                          Description = model.Description,
                          Rate = model.Amount,
                          ClubId = clubId)
    
    db.RemedyRates.Add(rate) |> ignore
    db.SaveChanges() |> ignore
    OkResult { model with Id = rate.Id }
    
let updateRemedyRate (club: Club) (ctx: HttpContext) (model: RemedyRate) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id
    query {
        for rate in db.RemedyRates do
        where (rate.Id = model.Id && rate.ClubId = clubId)
        select rate
    }
    |> Seq.tryHead
    |> function
        | Some rate ->
            rate.Description <- model.Description
            rate.Name <- model.Name
            rate.Rate <- model.Amount
            db.SaveChanges() |> ignore
            OkResult model 
        | None -> NotFound
    

    
let deleteRemedyRate (club : Club) fineId (db : Database) =
    let (ClubId clubId) = club.Id
    query {
        for rate in db.RemedyRates do
            where (rate.Id = fineId && rate.ClubId = clubId)
            select rate
    }    
    |> Seq.tryHead
    |> function 
    | None -> NotFound
    | Some rate -> 
        db.RemedyRates.Remove(rate) |> ignore
        db.SaveChanges() |> ignore
        OkResult()

