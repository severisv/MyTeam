namespace MyTeam

open Microsoft.AspNetCore.Antiforgery
open Giraffe

module Antiforgery =
    let getToken (ctx : HttpContext) =
        let antiforgery = ctx.GetService<IAntiforgery>()
        antiforgery.GetAndStoreTokens(ctx).RequestToken
