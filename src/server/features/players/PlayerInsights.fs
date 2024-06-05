module MyTeam.Players.Insights

open Giraffe
open MyTeam
open System.Linq


let get next (ctx: HttpContext) =

    let db = ctx.GetService<db.Tables.DataContext>()

    let result = db.Players.Select(fun m -> m.FirstName).ToList()

    json result next ctx
