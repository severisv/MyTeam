namespace MyTeam.Teams

open MyTeam
open MyTeam.Domain.Members
open Giraffe 

module Api =

    let list clubId db =
        Queries.list db clubId
        |> Ok