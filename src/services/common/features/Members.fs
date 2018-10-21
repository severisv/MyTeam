module MyTeam.Common.Features.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore
open System.Linq
open System

type MemberDetails =
    { Details : Member
      Phone : string
      Email : string
      BirthDate : DateTime option }
    member m.BirthYear = m.BirthDate |> Option.map (fun b -> b.Year)

let toRoleList (roleString : string) =
    if not <| isNull roleString && roleString.Length > 0 then 
        roleString.Split [| ',' |]
        |> Seq.map (Enums.parse<Role>)
        |> Seq.toList
    else []

let fromRoleList (roles : Role list) = System.String.Join(",", roles)

let selectMembers =
    fun (players : IQueryable<Models.Domain.Member>) -> 
        query { 
            for p in players do
                sortBy p.FirstName
                select 
                    (p.Id, p.FacebookId, p.FirstName, p.MiddleName, p.LastName, 
                     p.UrlName, p.ImageFull, p.Status)
        }
        |> Seq.map (fun (id, facebookId, firstName, middleName, lastName, urlName, imageFull, status) -> 
               { Id = id
                 FacebookId = facebookId
                 FirstName = firstName
                 MiddleName = middleName
                 LastName = lastName
                 UrlName = urlName
                 Image = imageFull
                 Status = enum<PlayerStatus> status })

let list : Database -> ClubId -> MemberWithTeamsAndRoles list =
    fun db clubId -> 
        let (ClubId clubId) = clubId
        db.Members.Where(fun p -> p.ClubId = clubId)
          .Include(fun p -> p.MemberTeams)
        |> Seq.map (fun p -> 
               { Details =
                     ({ Id = p.Id
                        FacebookId = Strings.defaultValue p.MiddleName
                        FirstName = p.FirstName
                        MiddleName = Strings.defaultValue p.MiddleName
                        LastName = p.LastName
                        Image = Strings.defaultValue p.MiddleName
                        UrlName = p.UrlName
                        Status = int p.Status |> enum<PlayerStatus> })
                 Teams =
                     p.MemberTeams
                     |> Seq.map (fun team -> team.TeamId)
                     |> Seq.toList
                 Roles = p.RolesString |> toRoleList })
        |> Seq.toList
        |> List.sortBy (fun p -> p.Details.FirstName)
