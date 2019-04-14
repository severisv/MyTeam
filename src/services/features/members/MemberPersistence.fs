module MyTeam.Members.Persistence

open Microsoft.EntityFrameworkCore
open MyTeam
open Shared
open MyTeam.Common.Features.Members
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Models.Domain

let setStatus : SetStatus =
    fun db clubId memberId status ->
        let members = Queries.members db clubId

        let memb =
            query { for m in members do
                    where (m.Id = memberId)
                    select m }
            |> Seq.head
        memb.Status <- (int status)
        db.SaveChanges() |> ignore
        UserId memb.UserName

let toggleRole : ToggleRole =
    fun db clubId memberId role ->
        let toggleRoleInList role roleList =
            if roleList |> List.contains role then roleList |> List.filter (fun r -> r <> role)
            else roleList |> List.append [ role ]

        let members = Queries.members db clubId

        let memb =
            query { for m in db.Members do
                    where (m.Id = memberId)
                    select m }
            |> Seq.head
        memb.RolesString <- memb.RolesString
                            |> toRoleList
                            |> toggleRoleInList role
                            |> fromRoleList
        db.SaveChanges() |> ignore
        UserId(memb.UserName =?? "")

let toggleTeam : ToggleTeam =
    fun db clubId memberId teamId ->
        let (ClubId clubId) = clubId

        let memb =            
            query { for m in db.Members.Include(fun m -> m.MemberTeams) do
                    where (m.Id = memberId)
                    select m }
            |> Seq.head
        if memb.ClubId <> clubId then
            failwith "Prøver å redigere spiller fra annen klubb - ingen tilgang"

        memb.MemberTeams
        |> Seq.tryFind (fun mt -> mt.TeamId = teamId)
        |> function
        | Some m -> db.Remove(m) |> ignore
        | None ->
            let memberTeam = MemberTeam()
            memberTeam.TeamId <- teamId
            memb.MemberTeams.Add(memberTeam)
        db.SaveChanges() |> ignore
