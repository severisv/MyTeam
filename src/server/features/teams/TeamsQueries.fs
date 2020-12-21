namespace MyTeam.Teams

open MyTeam
open Shared.Domain

module Queries =
       
    let list : ListTeams =
        fun db clubId ->
            let (ClubId clubId) = clubId
            query {
                for t in db.Teams do
                    where (t.ClubId = clubId)
                    select (t.Id, t.ShortName, t.Name, t.Formation)  }
            |> Seq.toList            
            |> List.map(fun (id, shortName, name, formation) -> 
                            {   Id = id
                                ShortName = shortName
                                Name = name
                                LeagueType = formation |> Clubs.asLeagueType  })
            |> List.sortBy(fun t -> t.ShortName)


