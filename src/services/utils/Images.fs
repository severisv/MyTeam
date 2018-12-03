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
              DefaultMember = opts.DefaultMember =?? ""
              DefaultArticle = opts.DefaultArticle =?? "" }
    
    let get ctx url getProps =
        let options = getOptions ctx
        Image.get options url getProps
    
    let getArticle ctx (getProps : GetProps) url =
        let options = getOptions ctx
        Image.getArticle options getProps url
    
    let getMember ctx (getProps : GetProps) url facebookId  =
        let options = getOptions ctx
        Image.getMember options getProps url facebookId 
