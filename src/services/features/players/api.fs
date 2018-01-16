namespace MyTeam

open MyTeam.Players
open Giraffe 
open MyTeam.Domain

module PlayerApi =

    let list (club : Club) (ctx: HttpContext) =
        Queries.getPlayers ctx.ConnectionString club.Id
        |> json, ctx

    let getFacebookIds (club : Club) (ctx: HttpContext) =
        Queries.getFacebookIds ctx.ConnectionString club.Id
        |> json, ctx