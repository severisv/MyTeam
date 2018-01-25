namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore

module Queries =

    let members connectionString clubId = 
        let db = Database.get connectionString 
        db.Dbo.Member
        |> Seq.filter(fun p -> p.ClubId = clubId), db

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
        fun connectionString clubId ->         
            let (ClubId clubId) = clubId 
            let (members, __) = members connectionString clubId 
            members
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.toList


