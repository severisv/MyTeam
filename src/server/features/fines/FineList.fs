module Server.Features.Fines.List

open System.Linq
open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Common
open Shared.Features.Fines.Common
open Client.Fines.List
open Shared.Strings


let view (club: Club) (user: User) (year: string option) (selectedMember: SelectedMember) (ctx: HttpContext) =

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
                    select (fine.Id, fine.MemberId, fine.Issued, fine.Amount, fine.RateName, fine.Comment)
            }
        | Year year ->
            query {
                for fine in db.Fines do
                    where (
                        fine.Rate.ClubId = clubId
                        && fine.Issued.Year = year
                    )

                    select (fine.Id, fine.MemberId, fine.Issued, fine.Amount, fine.RateName, fine.Comment)
            }
        |> Seq.toList

    let members =
        let memberIds =
            fines
            |> List.map (fun (_, memberId, _, _, _, _) -> memberId)

        query {
            for memb in db.Members do
                where (memberIds.Contains memb.Id)
        }
        |> Common.Features.Members.selectMembers
        |> Seq.toList

    let fines =
        fines
        |> List.map
            (fun (fineId, memberId, issued, amount, rateName, comment) ->
                { Id = fineId
                  Member = members |> List.find (fun m -> m.Id = memberId)
                  Amount = amount
                  Description = rateName
                  Comment = !!comment
                  Issued = issued })
        |> List.filter
            (fun m ->
                match selectedMember with
                | Member memberId -> m.Member.Id = memberId
                | AllMembers -> true)
        |> List.sortByDescending (fun fine -> fine.Issued)

    [ Client.view
          Client.Fines.List.containerId
          Client.Fines.List.element
          { Members = members
            SelectedMember = selectedMember
            User = user
            Path = ctx.Request.Path.Value
            Years = years
            Fines = fines
            ImageOptions = Images.getOptions ctx
            Year = year } ]
    |> layout club (Some user) (fun o -> { o with Title = "Bøter" }) ctx
    |> OkResult
