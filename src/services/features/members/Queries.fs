namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore
open MyTeam.Database

module Queries =

    let members (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Members |> Seq.filter(fun p -> p.ClubId = clubId)

    let list : ListMembers =
        fun db clubId ->
            let (ClubId clubId) = clubId
           
            db.Members.Include(fun p -> p.MemberTeams) 
            |> Seq.filter(fun p -> p.ClubId = clubId)
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


