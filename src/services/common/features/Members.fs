module MyTeam.Common.Features.Members

open MyTeam
open Shared.Domain
open Shared.Domain.Members
open System.Linq
open System
open Shared.Strings
open Microsoft.EntityFrameworkCore

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
                 Status = PlayerStatus.fromInt status })
        |> Seq.sortBy (fun p -> p.FirstName)


let list : Database -> ClubId -> MemberWithTeamsAndRoles list =
    fun db clubId -> 
        let (ClubId clubId) = clubId
        
        db.Members.Include(fun m -> m.MemberTeams)
            .Where(fun m -> m.ClubId = clubId)
            .ToList()
        |> Seq.toList
        |> List.map (fun m -> 
               { Details =
                     ({ Id = m.Id
                        FacebookId = !!m.FacebookId
                        FirstName = !!m.FirstName
                        MiddleName = !!m.MiddleName
                        LastName = !!m.LastName
                        Image = !!m.ImageFull
                        UrlName = !!m.UrlName
                        Status = PlayerStatus.fromInt m.Status})
                 Teams = m.MemberTeams |> Seq.toList |> List.map(fun mt -> mt.TeamId) |> List.filter((=) Guid.Empty >> not)                   
                 Roles = m.RolesString |> toRoleList })
        |> Seq.toList
        |> List.sortBy (fun p -> p.Details.FirstName)
