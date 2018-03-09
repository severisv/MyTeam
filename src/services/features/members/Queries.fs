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
            |> Seq.map(fun p ->                            
                            {
                                Details =  ({
                                                Id = p.Id
                                                FacebookId = p.FacebookId
                                                FirstName = p.FirstName
                                                MiddleName = p.MiddleName
                                                LastName = p.LastName
                                                Image = p.ImageFull
                                                UrlName = p.UrlName
                                                Status = int p.Status |> enum<Status> 
                                           })
                                Teams = p.MemberTeams 
                                           |> Seq.map(fun team -> team.TeamId)
                                           |> Seq.toList
                                Roles = p.RolesString |> toRoleList
                            }
                    )
            |> Seq.toList                

    let getFacebookIds : GetFacebookIds =
        fun db clubId ->         
            members db clubId
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.toList


