namespace MyTeam.Players

open MyTeam

module Queries =


    let getPlayers : GetPlayers =
        fun connectionString clubId ->

            let database = Database.get connectionString
   
            database.Dbo.Member
            |> Seq.filter (fun p -> p.ClubId = clubId)
            |> Seq.filter (fun p -> p.Discriminator = "Player")
            |> Seq.sortBy (fun p -> p.FirstName)
            |> Seq.map (fun p ->
                            {
                                Id = p.Id
                                FirstName = p.FirstName
                                LastName = p.LastName
                                MiddleName = p.MiddleName
                                UrlName = p.UrlName
                                Status = Status.Aktiv
                                Roles = []
                            }
                    )
            |> Seq.toList
