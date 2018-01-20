namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

module Queries =

    let members connectionString clubId = 
        let db = Database.get connectionString 
        db.Dbo.Member
        |> Seq.filter(fun p -> p.ClubId = clubId), db

    let list : ListMembers =
        fun connectionString clubId ->

            let database = Database.get connectionString
   
            let members = 
                    query {
                        for p in database.Dbo.Member do
                        where (p.ClubId = clubId && p.Discriminator = "Player")
                        join team in database.Dbo.MemberTeam on (p.Id = team.MemberId)
                        select (p, team)
                    }
                    |> Seq.toList

            let teams = members
                        |> Seq.map(fun (__, team) -> team)

            members 
            |> Seq.distinctBy(fun (p, __) -> p.Id)
            |> Seq.map(fun (p, __) -> 
                            {
                                Id = p.Id
                                FirstName = p.FirstName
                                LastName = p.LastName
                                MiddleName = p.MiddleName
                                UrlName = p.UrlName
                                Status = p.Status |> enum<Status> 
                                Roles = p.RolesString |> toRoleArray
                                TeamIds = teams |> Seq.filter(fun team -> team.MemberId = p.Id) 
                                                |> Seq.map(fun team -> team.TeamId)
                                                |> Seq.toList
                            }
                    )
            |> Seq.toList

    let getFacebookIds : GetFacebookIds =
        fun connectionString clubId ->            
            let (members, __) = members connectionString clubId 
            members
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.toList


