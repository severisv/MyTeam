namespace MyTeam.Players

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

module Queries =

    let players connectionString clubId = 
        let (ClubId clubId) = clubId
        let db = Database.get connectionString 
        db.Dbo.Member
        |> Seq.filter(fun p -> p.ClubId = clubId)        
        |> Seq.filter(fun p -> p.Discriminator = "Player"), db

    let list : ListPlayers =
        fun connectionString clubId ->
            let (ClubId clubId) = clubId

            let database = Database.get connectionString
   
            let players = 
                    query {
                        for p in database.Dbo.Member do
                        where (p.ClubId = clubId && p.Discriminator = "Player")
                        join team in database.Dbo.MemberTeam on (p.Id = team.MemberId)
                        select (p, team)
                    }
                    |> Seq.toList

            let teams = players
                        |> Seq.map(fun (__, team) -> team)

            players 
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
