namespace MyTeam

open MyTeam
open MyTeam.Domain

module Clubs = 
    type Get = HttpContext -> ClubIdentifier -> Option<Club>
    let get : Get =
        fun ctx clubId -> 
            let connectionString = getConnectionString ctx
            let database = Database.get connectionString

            let clubs =
                    query {
                        for club in database.Dbo.Club do
                        where (club.ClubIdentifier = clubId)
                        join team in database.Dbo.Team on (club.Id = team.ClubId)
                        select (club, team)
                    }
                    |> Seq.toList
                    
            let teams = clubs 
                        |> Seq.map(fun (__, team) -> 
                                    {
                                      Id = team.Id
                                      ShortName = team.ShortName
                                      Name = team.Name
                                    })
                        |> Seq.toList                                                    

            clubs 
            |> Seq.map(fun (club, __) -> 
                        {
                            Id = club.Id
                            ClubId = club.ClubIdentifier
                            ShortName = club.ShortName
                            Name = club.Name
                            Teams = teams
                            Favicon = club.Name
                            Logo = club.Logo
                        }) 
            |> Seq.tryHead                                  

    