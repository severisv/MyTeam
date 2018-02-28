namespace MyTeam

open Microsoft.AspNetCore.Antiforgery

module Antiforgery =
    let getToken  (ctx: HttpContext) = 
        let antiforgery = getService<IAntiforgery> ctx
        antiforgery.GetAndStoreTokens(ctx).RequestToken