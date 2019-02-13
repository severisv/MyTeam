namespace MyTeam.Teams

open MyTeam
open Shared
open Shared.Domain

module Queries =

    let teams (db: Database) clubId = 
        let (ClubId clubId) = clubId
        db.Teams
        |> Seq.filter(fun t -> t.ClubId = clubId)

    let list : ListTeams =
        fun db clubId ->

            teams db clubId
            |> Seq.map(fun t -> 
                            {
                                Id = t.Id
                                ShortName = t.ShortName
                                Name = t.Name                               
                            }
                    )
            |> Seq.toList
            |> List.sortBy(fun t -> t.ShortName)
