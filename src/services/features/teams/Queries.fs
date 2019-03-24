namespace MyTeam.Teams

open MyTeam
open Shared
open Shared.Domain

module Queries =
       

    let list : ListTeams =
        fun db clubId ->
            let (ClubId clubId) = clubId
            query {
                for t in db.Teams do
                    where (t.ClubId = clubId)
                    select (t.Id, t.ShortName, t.Name)  }
            |> Seq.toList            
            |> List.map(fun (id, shortName, name) -> 
                            {   Id = id
                                ShortName = shortName
                                Name = name  })
            |> List.sortBy(fun t -> t.ShortName)
