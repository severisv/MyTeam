namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore
open System.Linq

module Queries =

    let members (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Members.Where(fun p -> p.ClubId = clubId)

    let list : ListMembers =
        fun db clubId ->         
            (members db clubId).Include(fun p -> p.MemberTeams) 
            |> Seq.toList
            |> List.map(fun p -> 
                            {
                                Id = p.Id
                                FirstName = p.FirstName
                                LastName = p.LastName
                                MiddleName = p.MiddleName
                                UrlName = p.UrlName
                                Status = int p.Status |> enum<Status> 
                                Roles = p.RolesString |> toRoleList
                                TeamIds = p.MemberTeams 
                                                |> Seq.map(fun team -> team.TeamId)
                                                |> Seq.toList
                            }
                    )

    let getFacebookIds : GetFacebookIds =
        fun db clubId ->         
            members db clubId
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.toList


