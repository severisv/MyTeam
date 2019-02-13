namespace MyTeam.Teams

open MyTeam
open Shared
open Shared.Domain.Members
open Giraffe 

module Api =

    let list clubId db =
        Queries.list db clubId
        |> OkResult