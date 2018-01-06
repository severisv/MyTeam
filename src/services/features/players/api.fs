namespace Services

open Services.Players
open Queries
open Giraffe 

module PlayerApi =

    let list connectionString clubId =
        getPlayers connectionString clubId
        |> json