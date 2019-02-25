module Server.Features.Fines.List

open System
open System.Linq
open Giraffe.GiraffeViewEngine
open Shared.Domain
open Shared.Domain.Members
open Shared
open MyTeam
open MyTeam.Views
open Shared.Components
open Currency
open Common
open Tables
open Fable.Helpers
open Fable.Helpers.React.Props
open MyTeam
open MyTeam.Common.Features
open Shared.Features.Fines


let view (club : Club) (user : User) (year : string option) (selectedMember: SelectedMember) (ctx : HttpContext) =

     let db = ctx.Database
     let (ClubId clubId) = club.Id
     
     let years = getYears db club.Id
     let year = getSelectedYear year years

         
     let fines =
         match year with
         | AllYears -> 
             query {
                 for fine in db.Fines do
                     where (fine.Rate.ClubId = clubId)
                     select (fine.Id, fine.MemberId, fine.Issued, fine.Amount,  fine.RateName)
             }
         | Year year ->
             query {
                 for fine in db.Fines do
                     where (fine.Rate.ClubId = clubId && fine.Issued.Year = year)
                     select (fine.Id, fine.MemberId, fine.Issued, fine.Amount, fine.RateName)
             }
         |> Seq.toList
    
     let members =
         let memberIds = fines |> List.map (fun (_, memberId, _, _ ,_) -> memberId)
         query {
             for memb in db.Members do
                 where (memberIds.Contains memb.Id) }
         |> Common.Features.Members.selectMembers
         |> Seq.toList               
    
     let fines =
         fines
         |> List.map (fun (fineId, memberId, issued, amount, rateName) ->
             { Id = fineId
               Member = members |> List.find (fun m -> m.Id = memberId) 
               Amount = amount
               Description = rateName
               Issued = issued })
         |> List.filter (fun m ->
             match selectedMember with
             | Member memberId -> m.Member.Id = memberId
             | AllMembers -> true)
         |> List.sortByDescending (fun fine -> fine.Issued)
        
            
     let isSelected year selectedMember =
        (=) (createUrl year selectedMember) 
        
     [
      Client.view listView {
                Members = members
                SelectedMember = selectedMember
                User = user
                Path = ctx.Request.Path.Value
                Years = years
                Fines = fines
                ImageOptions = Images.getOptions ctx
                Year = year }
     ]
     |> layout club (Some user) (fun o -> { o with Title = "Bøter" }) ctx
     |> OkResult
