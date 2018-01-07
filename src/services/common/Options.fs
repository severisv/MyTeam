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
    // let getOptions<'TOptions> (ctx : HttpContext) : 'T =
    //         let connectionOptions = ctx.RequestServices.GetService<IOptions<'T>>()
    //         connectionOptions.Value

    let getConnectionString (ctx: HttpContext) = 
            ctx.RequestServices.GetService<IOptions<ConnectionStrings>>().Value.DefaultConnection