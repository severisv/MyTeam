namespace MyTeam

open Microsoft.Extensions.Options
open Giraffe
open Image

module Images =
    let getSecretOptions (ctx : HttpContext) =
        (ctx.GetService<IOptions<CloudinarySettings>>()).Value
    
    let getOptions (ctx : HttpContext) =
        getSecretOptions ctx
        |> fun opts -> 
            { CloudName = opts.CloudName
              DefaultMember = opts.DefaultMember
              DefaultArticle = opts.DefaultArticle }
    
    let get ctx url getProps =
        let options = getOptions ctx
        Image.get options url getProps
    
    let getArticle ctx url (getProps : GetProps) =
        let options = getOptions ctx
        Image.getArticle options url getProps
    
    let getMember ctx url facebookId (getProps : GetProps) =
        let options = getOptions ctx
        Image.getMember options url facebookId getProps
