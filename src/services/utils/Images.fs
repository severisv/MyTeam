namespace MyTeam

open Microsoft.Extensions.Options
open Microsoft.Extensions.ObjectPool


module ImagesInternal =
    
    type Format = 
        | Jpg

    type ImageProperties = {
        Width: int option
        Height: int option
        Quality: int
        Format: Format option
    }

    type GetProps = ImageProperties -> ImageProperties

    let defaultArticle = ""
    let baseUrl cloudName = sprintf "http://res.cloudinary.com/%s/" cloudName

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
            let extension = (Enums.toString extension).ToLower()
            [".png";".PNG";".bmp";".BMP";".tiff";".TIFF"]
            |> List.fold (fun (acc: string) elem -> acc.Replace(elem, extension)) url

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
  
    let get (ctx: HttpContext) url getProps =
        let options = (getService<IOptions<CloudinarySettings>> ctx).Value
        let baseUrl = baseUrl options.CloudName

        createImageUrl baseUrl url getProps

    let defaultArticle ctx getProps = 
        get ctx "image/upload/v1448309373/article_default_hnwnxo.jpg" getProps

    let defaultMember ctx getProps = 
        get ctx "image/upload/v1448559418/default_player_dnwac0.gif" getProps

    let getMember ctx url facebookId (getProps: GetProps) =
        
        let isComplete (url: string) = url.StartsWith("http")

        match (url, facebookId) with
        | (url, _) when url |> isComplete -> url
        | (url, _) when url |> hasValue -> get ctx url getProps
        | (_, id) when id |> hasValue -> getFacebookImage id getProps
        | _ -> defaultMember ctx getProps

 