namespace Shared

open Shared.Strings

type Format = Jpg

module Image =
    type CloudinaryOptions =
        { CloudName : string
          DefaultMember : string
          DefaultArticle : string }
    
    type ImageProperties =
        { Width : int option
          Height : int option
          Quality : int
          Format : Format option }
    
    type GetProps = ImageProperties -> ImageProperties
    
    let private baseUrl cloudName = sprintf "https://res.cloudinary.com/%s/" cloudName
    
    let private defaultProps =
        { Width = None
          Height = None
          Quality = 100
          Format = None }
    
    let get options getProps url =
        let baseUrl = baseUrl options.CloudName
        
        let createImageUrl baseUrl (imageUrl : string) (getProps : GetProps) =
            let props = getProps defaultProps
            
            let urlProps =
                let toUrlProp letter prop =
                    prop |> Option.fold (fun _ v -> sprintf ",%s_%i" letter v) ""
                sprintf "c_fill%s%s%s" (props.Height |> toUrlProp "h") 
                    (props.Width |> toUrlProp "w") (Some props.Quality |> toUrlProp "q")
            
            let replaceExtension extension (url : string) =
                [ ".png"; ".PNG"; ".bmp"; ".BMP"; ".tiff"; ".TIFF" ]
                |> List.fold (fun (acc : string) elem -> 
                       acc.Replace(elem, 
                                   sprintf ".%O" extension
                                   |> string
                                   |> Strings.toLower)) url
            
            let url = baseUrl + imageUrl.Replace("/upload", sprintf "/upload/%s" urlProps)
            props.Format |> Option.fold (fun _ format -> url |> replaceExtension format) url
        createImageUrl baseUrl url getProps
    
    let getArticle options (getProps : GetProps) url =
        let defaultArticle options getProps =
            let imageUrl =
                if Strings.hasValue options.DefaultMember then options.DefaultMember
                else "image/upload/v1448309373/article_default_hnwnxo.jpg"
            get options getProps imageUrl
        match url with
        | value when Strings.hasValue value -> get options getProps value
        | _ -> defaultArticle options getProps
    
    let getMember options (getProps : GetProps) url facebookId  =
        let defaultMember options getProps =
            let imageUrl =
                if hasValue options.DefaultMember then options.DefaultMember
                else "image/upload/v1603632562/undraw_junior_soccer_6sop_eylk33.png"
            get options getProps imageUrl
        
        let isComplete(url : string) = url
                                       |> hasValue
                                       && url.StartsWith("http")
        
        let getFacebookImage facebookId getProps =
            let props = getProps defaultProps
            
            let size =
                match props.Width with
                | Some w when w < 51 -> "square"
                | Some w when w < 101 -> "normal"
                | _ -> "large"
            sprintf "https://graph.facebook.com/%s/picture?type=%s" facebookId size
        match (url, facebookId) with
        | (url, _) when url |> isComplete -> url
        | (url, _) when url |> hasValue -> get options getProps url
        | (_, id) when id |> hasValue -> getFacebookImage id getProps
        | _ -> defaultMember options getProps
