namespace MyTeam

open Microsoft.Extensions.Options
open Giraffe

type Format = 
    | Jpg

module ImagesInternal =
    


    type ImageProperties = {
        Width: int option
        Height: int option
        Quality: int
        Format: Format option
    }

    type GetProps = ImageProperties -> ImageProperties

    let defaultArticle = ""
    let baseUrl cloudName = sprintf "https://res.cloudinary.com/%s/" cloudName

    let defaultProps = {
            Width = None
            Height = None
            Quality = 100
            Format = None
        }

    let createImageUrl baseUrl (imageUrl: string) (getProps: GetProps) =        

        let props = getProps defaultProps
        let urlProps =
                let toUrlProp letter prop = 
                    prop |> Option.fold (fun _ v -> sprintf ",%s_%i" letter v) ""
                
                sprintf 
                    "c_fill%s%s%s" 
                    (props.Height |> toUrlProp "h") 
                    (props.Width |> toUrlProp "w" )
                    (Some props.Quality |> toUrlProp "q")


        let replaceExtension extension (url: string) =
            [".png";".PNG";".bmp";".BMP";".tiff";".TIFF"]
            |> List.fold (fun (acc: string) elem -> acc.Replace(elem, sprintf ".%O" extension |> toLowerString)) url

        let url = 
            baseUrl +
            imageUrl.Replace("/upload", sprintf "/upload/%s" urlProps)
        
        props.Format 
            |> Option.fold 
                (fun _ format -> url |> replaceExtension format) 
                url
            
        
        

    let getFacebookImage facebookId getProps =
        let props = getProps defaultProps

        let size = 
            match props.Width with
            | Some w when w < 51 -> "square"
            | Some w when w < 101 -> "normal"
            | _ -> "large"      

        sprintf "https://graph.facebook.com/%s/picture?type=%s" facebookId size



open ImagesInternal

module Images =
  
    let private getOptions (ctx: HttpContext) =
        (ctx.GetService<IOptions<CloudinarySettings>>()).Value

    let get ctx url getProps =
        let options = getOptions ctx
        let baseUrl = baseUrl options.CloudName

        createImageUrl baseUrl url getProps

    let private defaultArticle ctx getProps = 
        let options = getOptions ctx
        let imageUrl = 
            if hasValue options.DefaultMember then options.DefaultMember
            else "image/upload/v1448309373/article_default_hnwnxo.jpg"
        
        get ctx imageUrl getProps

    
    let getArticle ctx url (getProps: GetProps) =
        match url with
        | value when hasValue value -> get ctx value getProps
        | _ -> defaultArticle ctx getProps

    let private defaultMember ctx getProps = 
        let options = getOptions ctx
        let imageUrl = 
            if hasValue options.DefaultMember then options.DefaultMember
            else "image/upload/v1448559418/default_player_dnwac0.gif"

        get ctx imageUrl getProps

    let getMember ctx url facebookId (getProps: GetProps) =
        
        let isComplete (url: string) = url |> hasValue && url.StartsWith("http")

        match (url, facebookId) with
        | (url, _) when url |> isComplete -> url
        | (url, _) when url |> hasValue -> get ctx url getProps
        | (_, id) when id |> hasValue -> getFacebookImage id getProps
        | _ -> defaultMember ctx getProps

 