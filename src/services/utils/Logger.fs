module MyTeam.Logger

open MyTeam
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection;

let get<'T> (ctx: HttpContext) =
    ctx.RequestServices.GetService<ILogger<'T>>()