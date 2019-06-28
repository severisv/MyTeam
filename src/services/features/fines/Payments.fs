module Server.Features.Fines.Payments

open System.Linq
open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Common
open Shared
open Shared.Features.Fines.Common
open Shared.Features.Fines.Payments
open Strings

let view (club : Club) (user : User) (year : string option) (selectedMember: SelectedMember) (ctx : HttpContext) =

     let db = ctx.Database
     let (ClubId clubId) = club.Id
     
     let years =
         query {
               for payment in db.Payments do
                   where (payment.ClubId = clubId)
                   select payment.TimeStamp.Year
                   distinct
           }
           |> Seq.toList
           |> List.sortDescending
           
     let year = getSelectedYear year years
         
     let payments =
         match year with
         | AllYears -> 
             query {
                 for payment in db.Payments do
                     where (payment.ClubId = clubId)
                     select (payment.Id, payment.MemberId, payment.TimeStamp, payment.Amount, payment.Comment)
             }
         | Year year ->
             query {
                 for payment in db.Payments do
                     where (payment.ClubId = clubId && payment.TimeStamp.Year = year)
                     select (payment.Id, payment.MemberId, payment.TimeStamp, payment.Amount, payment.Comment)
             }
         |> Seq.toList
    
     let members =
         let memberIds = payments |> List.map (fun (_, memberId, _, _ ,_) -> memberId)
         query {
             for memb in db.Members do
                 where (memberIds.Contains memb.Id) }
         |> Common.Features.Members.selectMembers
         |> Seq.toList               
    
     let payments =
         payments
         |> List.map (fun (fineId, memberId, issued, amount, comment) ->
             { Id = fineId
               Member = members |> List.find (fun m -> m.Id = memberId) 
               Amount = amount
               Comment = !!comment
               Date = issued })
         |> List.filter (fun m ->
             match selectedMember with
             | Member memberId -> m.Member.Id = memberId
             | AllMembers -> true)
         |> List.sortByDescending (fun payment -> payment.Date)
         
     
     let paymentInformation =
         query {
             for pi in db.PaymentInformation do
                 where (pi.ClubId = clubId)
                 select pi.Info
         } |> Seq.tryHead
           |> Option.defaultValue ""
     
     [ Client.view
           paymentsView           
           Client.Fines.Payments.element
             {
                Members = members
                SelectedMember = selectedMember
                User = user
                Path = ctx.Request.Path.Value
                Years = years
                Payments = payments
                ImageOptions = Images.getOptions ctx
                Year = year
                PaymentInformation = paymentInformation }]     
     |> layout club (Some user) (fun o -> { o with Title = "Innbetalinger" }) ctx
     |> OkResult
