namespace MyTeam.Teams

open MyTeam
open MyTeam.Domain.Members
open Giraffe 

module Api =

    let list clubId (ctx: HttpContext) =
        Queries.list ctx.Database clubId
        |> json