module MyTeam.Request

open MyTeam

let isJson (ctx: HttpContext) =              
    ctx.Request.Headers.TryGetValue("Accept")
    |> function
        | (true, s) when s |> Seq.exists (fun s -> s = "application/json") -> true
        | _ -> false      

