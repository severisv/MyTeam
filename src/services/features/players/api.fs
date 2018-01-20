namespace MyTeam

open MyTeam.Players
open MyTeam.Domain.Members
open Giraffe 

module PlayerApi =

    let list clubId (ctx: HttpContext) =
        Queries.getPlayers ctx.ConnectionString clubId
        |> json, ctx