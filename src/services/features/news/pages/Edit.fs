module MyTeam.News.Pages.Edit

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Validation
open MyTeam.Domain
open MyTeam.News
open MyTeam.News.Pages
open MyTeam.Views
open MyTeam.Shared.Components
open Microsoft.Extensions.Options


let private editView (ctx: HttpContext) (club: Club) user name (article: ArticleModel) published validationErrors =
    let db = ctx.Database
    let cloudinarySettings = ctx.GetService<IOptions<CloudinarySettings>>()
    let latestGames = Queries.listRecentGames db club.Id published
    [
        main [] [
            block [] [
                form [ _action <| sprintf "/nyheter/endre/%s" name; _method "POST" ] [                         
                    div [ _class "cloudinary-preview news-imageWrapper" ][ 
                        img [ _src <| Components.image ctx article.ImageUrl ] 
                        ]
                    input [ _name "imageUrl"; _type "hidden"; _value article.ImageUrl]

                    div [ _class "form-file-upload-wrapper btn btn-default" ] [ 
                        input [ _name "file"; _type "file"; _class "cloudinary-fileupload pull-left"; attr "data-cloudinary-field" "imageUrl" ]
                      ]
                    a [ _class "btn btn-danger pull-right confirm-dialog"; _href "delete"; attr "data-message" "Er du sikker på at du vil slette?" ][ 
                        !!Icons.delete
                      ]
                    div [ _class "clearfix" ] []
                    br [] 
                    ul [ _class "text-danger" ] 
                        (validationErrors
                        |> List.collect (fun error -> error.Errors 
                                                    |> List.map (fun e ->   li [] [encodedText <| sprintf "%s: %s" error.Name e] )))
                    div [ _class "form-group news-matchreportSelect" ] [ 
                        label [ _class "col-xs-3 col-sm-2 no-padding control-label" ] [ encodedText "Kamprapport?" ]
                        div [ _class "col-xs-9 col-sm-10 flex" ][ 
                            div [][ 
                                input [ 
                                    _name "IsMatchReport"
                                    _type "checkbox"
                                    _class "form-control";
                                    article.IsMatchReport =? (_checked,_empty)
                                    attr "onclick" "window.checkbox.showHideAssociatedElement(this, '#GameIdWrapper')" ]
                              ]
                            div [ _class "flex-2" ] [ 
                                span [ _id "GameIdWrapper"; article.IsMatchReport =? (_empty, _hidden) ] [
                                   select [ 
                                        _name "GameId"
                                        _class "form-control" ]
                                        ([ 
                                           option [ _value "" ][ encodedText "- Velg kamp -" ]                                             
                                        ] @
                                           (latestGames 
                                           |> List.map (fun game -> 
                                                option [ attr "selected" <| string (Some game.Id = article.GameId); _value <| string game.Id ][ encodedText game.Name ]
                                           )))
                                      ]
                                  ]
                              ]
                          ]                      
                    br []
                    br []                     
                    label [ _name "Headline" ] [ encodedText "Overskrift:" ]
                    div [ _class "form-group" ][ 
                        input [ _class "form-control"; _name "Headline"; _placeholder "Sensasjonell snuoperasjon av Sleivdal"; _value article.Headline ]
                      ]
                    div [ _class "form-group" ] [
                        textarea [ _name "Content"; _class "form-control update-table tinymce"; _placeholder "Innhold" ] [ rawText article.Content ]
                      ]
                    div [ _class "form-group" ][ 
                        button [ _type "submit"; _class "btn btn-primary" ] [ 
                            encodedText "Lagre" ]
                      ]
                    ]          
            ]          
        ]          
        sidebar [] [
            Components.articleNav db club
        ]    
    ]
    |> layout club (Some user) (fun o -> { o with 
                                            Title = "Rediger artikkel"
                                            Scripts = Components.tinyMceScripts @ Components.cloudinaryScripts cloudinarySettings.Value
                                    }
                        ) ctx
    |> Ok


let view (club: Club) user name (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
    | None -> Error NotFound
    | Some article ->    

        ({ 
            IsMatchReport = article.GameId.IsSome
            GameId = article.GameId
            Headline = article.Details.Headline
            Content = article.Content
            ImageUrl = article.Details.Image    
        }, article.Details.Published)
        |> fun (article, published) ->
          
            editView ctx club user name article published []

let post (club: Club) (user: Users.User) name (ctx: HttpContext) =

    let db = ctx.Database
 
    let form = ctx.BindForm<ArticleModel>()
        
    form ==>
        [        
           <@ form.Headline @> >- [isRequired]
           <@ form.Content @> >- [isRequired]
        ]
        |> function
        | Ok form -> 
            Persistence.saveArticle db club.Id name form
            |> Result.bind (fun _ -> 
                Error (Redirect <| sprintf "/nyheter/vis/%s" name))
            

        | Error validationErrors ->
            Queries.getArticle db club.Id name
            |> function
            | None -> Error NotFound
            | Some article ->    
                (form, article.Details.Published)
                |> fun (article, published) ->
                  
                    editView ctx club user name article published validationErrors
