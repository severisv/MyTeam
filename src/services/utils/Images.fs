namespace MyTeam

open Microsoft.Extensions.Options
open Giraffe

open Image

module Images =
  
    let getOptions (ctx: HttpContext) =
        (ctx.GetService<IOptions<CloudinarySettings>>()).Value

    let get ctx url getProps =
        let options = getOptions ctx
        Image.get options url getProps

    
    let getArticle ctx url (getProps: GetProps) =
        let options = getOptions ctx
        Image.getArticle options url getProps
        
    let getMember ctx url facebookId (getProps: GetProps) =
        let options = getOptions ctx
        Image.getMember options url facebookId getProps

 