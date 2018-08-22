module MyTeam.Logger

open MyTeam
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection;

let get (ctx: HttpContext) =
    ctx.RequestServices.GetService<ILogger<HttpContext>>()