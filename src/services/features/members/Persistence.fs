namespace MyTeam.Members

open MyTeam

module Persistence =
    let setStatus : SetStatus =
        fun connectionString clubId memberId status -> 
            let (members, db) = Queries.members connectionString clubId
            members
            |> Seq.filter(fun p -> p.Id = memberId)
            |> Seq.iter(fun p ->
                p.Status <- int status
            )
            db.SubmitUpdates()

