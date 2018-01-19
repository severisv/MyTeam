namespace MyTeam.Players

open MyTeam
open MyTeam.Domain.Members

module Persistence =
    let setStatus : SetStatus =
        fun connectionString playerId status -> 
            let database = Database.get connectionString
            ()

