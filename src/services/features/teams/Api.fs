namespace MyTeam

open MyTeam.Teams
open MyTeam.Domain.Members
open Giraffe 

module TeamApi =

    let list clubId (ctx: HttpContext) =
        Queries.list ctx.ConnectionString clubId
        |> json