module MyTeam.Common.Features.Members

open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open Microsoft.EntityFrameworkCore
open System.Linq
open System
open Shared.Strings

type MemberDetails =
    { Details : Member
      Phone : string
      Email : string
      BirthDate : DateTime option }
    member m.BirthYear = m.BirthDate |> Option.map (fun b -> b.Year)

let toRoleList (roleString : string) =
    if not <| isNull roleString && roleString.Length > 0 then 
        roleString.Split [| ',' |]
        |> Seq.map Enums.fromString<Role>
        |> Seq.toList
    else []

let fromRoleList (roles : Role list) = System.String.Join(",", roles)

let selectMembers =
    fun (players : IQueryable<Models.Domain.Member>) -> 
        query { 
            for p in players do
                select 
                    (p.Id, p.FacebookId, p.FirstName, p.MiddleName, p.LastName, 
                     p.UrlName, p.ImageFull, p.Status)
        }
        |> Seq.map (fun (id, facebookId, firstName, middleName, lastName, urlName, imageFull, status) -> 
               { Id = id
                 FacebookId = !!facebookId
                 FirstName = !!firstName
                 MiddleName = !!middleName
                 LastName = !!lastName
                 UrlName = !!urlName
                 Image = !!imageFull
                 Status = statusFromInt status })
        |> Seq.sortBy (fun p -> p.FirstName)


let list : Database -> ClubId -> MemberWithTeamsAndRoles list =
    fun db clubId -> 
        let (ClubId clubId) = clubId
        query {
            for m in db.Members do
            where (m.ClubId = clubId)
            groupJoin memberTeam in db.MemberTeams on (m.Id = memberTeam.MemberId) into result
            for mt in result.DefaultIfEmpty() do 
            select ((m.Id, m.FacebookId, m.FirstName, m.MiddleName, m.LastName, m.ImageFull, m.UrlName, m.Status, m.RolesString), mt.TeamId)
        }
        |> Seq.toList
        |> List.groupBy (fun (m, _) -> m)
        |> List.map(fun (key, values) -> (key, values |> List.map(fun (m, teamId) -> teamId)))
        |> List.map (fun ((id, facebookId, firstName, middleName, lastName, image, urlName, status, rolesString), teamIds) -> 
               { Details =
                     ({ Id = id
                        FacebookId = !!facebookId
                        FirstName = !!firstName
                        MiddleName = !!middleName
                        LastName = !!lastName
                        Image = !!image
                        UrlName = !!urlName
                        Status = statusFromInt status})
                 Teams = teamIds |> List.filter((=) Guid.Empty >> not)                   
                 Roles = rolesString |> toRoleList })
        |> Seq.toList
        |> List.sortBy (fun p -> p.Details.FirstName)
