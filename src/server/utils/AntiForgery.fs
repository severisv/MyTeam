module MyTeam.Antiforgery

open Microsoft.AspNetCore.Antiforgery
open Giraffe


let getToken (ctx : HttpContext) =
    let antiforgery = ctx.GetService<IAntiforgery>()
    antiforgery.GetAndStoreTokens(ctx).RequestToken


let validate =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let antiforgery = ctx.GetService<IAntiforgery>()
                
            let! isValid = antiforgery.IsRequestValidAsync ctx

            if isValid then
                return! next ctx
            else return! failwith "Invalid antiforgery token"    
        }