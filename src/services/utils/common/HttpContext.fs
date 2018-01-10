namespace MyTeam

open Microsoft.Extensions.DependencyInjection;
open Microsoft.AspNetCore.Http

[<AutoOpen>]
module HttpContextExtensions =
 
    let getService<'T> (ctx: HttpContext) =             
        ctx.RequestServices.GetService<'T>()
