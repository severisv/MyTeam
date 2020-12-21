module MyTeam.Request

open MyTeam
open Shared

let isJson (ctx : HttpContext) =
    ctx.Request.Headers.TryGetValue("Accept")
    |> function 
    | (true, s) when s |> Seq.exists (fun s -> s = "application/json") -> true
    | _ -> false

let fullPath (ctx : HttpContext) =
    (string ctx.Request.Path) + (string ctx.Request.QueryString)
