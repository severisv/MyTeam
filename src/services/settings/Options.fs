namespace MyTeam

open Microsoft.Extensions.DependencyInjection;
open Microsoft.Extensions.Options
open Microsoft.AspNetCore.Http

[<CLIMutable>]
type ConnectionStrings = {
    DefaultConnection: string

} 

[<CLIMutable>]
type FacebookOptions = {
  AppId: string
}


[<AutoOpen>]
module Options =
 
    let getConnectionString (ctx: HttpContext) = 
            ctx.RequestServices.GetService<IOptions<ConnectionStrings>>().Value.DefaultConnection
