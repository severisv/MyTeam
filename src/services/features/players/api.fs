namespace Services.Players

open Giraffe
open Queries

module Api =

    let players teamId =
        getPlayers teamId
        |> json