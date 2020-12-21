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

type CloudinarySettings() =
    member val ApiKey = Unchecked.defaultof<string> with get, set
    member val ApiSecret = Unchecked.defaultof<string> with get, set
    member val CloudName = Unchecked.defaultof<string> with get, set
    member val DefaultMember = Unchecked.defaultof<string> with get, set
    member val DefaultArticle = Unchecked.defaultof<string> with get, set

type AssetHashes() =
    member val FableJs = Unchecked.defaultof<string> with get, set
    member val MainJs = Unchecked.defaultof<string> with get, set
    member val LibJs = Unchecked.defaultof<string> with get, set
    member val MainCss = Unchecked.defaultof<string> with get, set

[<AutoOpen>]
module Options =
 
    let getConnectionString (ctx: HttpContext) = 
            ctx.RequestServices.GetService<IOptions<ConnectionStrings>>().Value.DefaultConnection
