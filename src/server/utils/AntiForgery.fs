module MyTeam.Antiforgery

open Microsoft.AspNetCore.Antiforgery
open Giraffe


let getToken (ctx: HttpContext) =
    let antiforgery = ctx.GetService<IAntiforgery>()
    antiforgery.GetAndStoreTokens(ctx).RequestToken


let validate =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let antiforgery = ctx.GetService<IAntiforgery>()

            let! isValid = antiforgery.IsRequestValidAsync ctx

            if isValid then
                return! next ctx


            else
                let clientDetails =
                    $"""Referer: {(string ctx.Request.Headers.["Referer"])} \n{(ctx.Request.Headers
                                                                                |> Seq.filter (fun kv -> not <| kv.Key.ToLower().Contains "cookie")
                                                                                |> Seq.map (fun keyValue -> $"%s{keyValue.Key}: %s{string keyValue.Value}")
                                                                                |> String.concat ",\n  ")}"""



                return! failwithf "Invalid antiforgery token\n%O" clientDetails
        }
