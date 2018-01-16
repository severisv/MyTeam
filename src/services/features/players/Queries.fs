namespace MyTeam.Players

open MyTeam
open MyTeam.Domain.Members

module Queries =

    let getPlayers : GetPlayers =
        fun connectionString clubId ->

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

    let getFacebookIds : GetFacebookIds =
        fun connectionString clubId ->
            let database = Database.get connectionString
            
            database.Dbo.Member 
            |> Seq.filter (fun m -> m.ClubId = clubId)
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.toList


