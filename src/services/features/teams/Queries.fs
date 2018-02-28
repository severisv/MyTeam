namespace MyTeam.Teams

open MyTeam
open MyTeam.Domain

module Queries =

    let teams connectionString clubId = 
        let (ClubId clubId) = clubId
        let db = Db.get connectionString 
        db.Dbo.Team
        |> Seq.filter(fun t -> t.ClubId = clubId), db

    let list : ListTeams =
        fun connectionString clubId ->

            let (teams, db) = teams connectionString clubId 

            teams
            |> Seq.map(fun t -> 
                            {
                                Id = t.Id
                                ShortName = t.ShortName
                                Name = t.Name                               
                            }
                    )
            |> Seq.toList
            |> List.sortBy(fun t -> t.ShortName)
