namespace MyTeam

open Microsoft.Extensions.DependencyInjection;
open Microsoft.Extensions.Options
open Microsoft.AspNetCore.Http

[<CLIMutable>]
type ConnectionStrings = {
    DefaultConnection: string
}

[<AutoOpen>]
module Options =
 
    let getConnectionString (ctx: HttpContext) = 
            ctx.RequestServices.GetService<IOptions<ConnectionStrings>>().Value.DefaultConnection

    let getService<'T> (ctx: HttpContext) =             
        ctx.RequestServices.GetService<'T>()
