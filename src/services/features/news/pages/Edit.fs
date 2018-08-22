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
open MyTeam.Common.News


let private editView (ctx: HttpContext) (club: Club) user name (article: ArticleModel) published validationErrors =
    let db = ctx.Database
    let cloudinarySettings = ctx.GetService<IOptions<CloudinarySettings>>()
    let latestGames = Queries.listRecentGames db club.Id published

    [
        mtMain [] [
            block [] [
                form [  _action (name |> Option.map (fun name -> sprintf "/nyheter/endre/%s" name) 
                                      |> Option.defaultValue "/nyheter/ny") 
                        _method "POST" ] [                         
                    div [ _class "cloudinary-preview news-imageWrapper" ][ 
                        img [ _src <| Components.image ctx article.ImageUrl ] 
                        ]
                    input [ _name "imageUrl"; _type "hidden"; _value article.ImageUrl]

                    div [ _class "form-file-upload-wrapper btn btn-default" ] [ 
                        input [ _name "file"; _type "file"; _class "cloudinary-fileupload pull-left"; attr "data-cloudinary-field" "imageUrl" ]
                      ]
                    name |> Option.map (fun name -> 
                                                a [ _class "btn btn-danger pull-right confirm-dialog"; _href <| sprintf "/nyheter/slett/%s" name; attr "data-message" "Er du sikker på at du vil slette artikkelen?" ][ 
                                                    !!Icons.delete
                                                  ]
                                        )
                         |> Option.defaultValue empty                                    
                    
                    div [ _class "clearfix" ] []
                    br [] 
                    ul [ _class "text-danger" ] 
                        (validationErrors
                        |> List.collect (fun error -> error.Errors 
                                                    |> List.map (fun e -> li [] [encodedText <| sprintf "%s: %s" error.Name e] )))
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
                                            Title = name |> Option.map(fun _ -> "Rediger artikkel") |> Option.defaultValue "Skriv ny artikkel"
                                            Scripts = Components.tinyMceScripts @ Components.cloudinaryScripts cloudinarySettings.Value
                                    }
                        ) ctx
    |> OkResult


let view (club: Club) user name (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
    | None -> NotFound
    | Some article ->    

        ({ 
            IsMatchReport = article.GameId.IsSome
            GameId = article.GameId
            Headline = article.Details.Headline
            Content = article.Content
            ImageUrl = article.Details.Image    
        }, article.Details.Published)
        |> fun (article, published) ->
          
            editView ctx club user (Some name) article published []

let editPost (club: Club) (user: Users.User) name (ctx: HttpContext) =

    let db = ctx.Database
 
    let form = ctx.BindForm<ArticleModel>()
        
    { form with 
        IsMatchReport = (string ctx.Request.Form.["IsMatchReport"]) = "on" // Workaround
    } ==>
        [        
           <@ form.Headline @> >- [isRequired]
           <@ form.Content @> >- [isRequired]
        ]
        |> function
        | Ok form -> 
            Persistence.saveArticle db club.Id name form
            |> Results.bind (fun _ -> 
                Redirect <| sprintf "/nyheter/vis/%s" name)
            

        | Error validationErrors ->
            Queries.getArticle db club.Id name
            |> function
            | None -> NotFound
            | Some article ->    
                (form, article.Details.Published)
                |> fun (article, published) ->
                    editView ctx club user (Some name) article published validationErrors


let create (club: Club) user (ctx: HttpContext) =    
    ({ 
        IsMatchReport = false
        GameId = None
        Headline = ""
        Content = ""
        ImageUrl = ""    
    }, System.DateTime.Now)
    |> fun (article, published) ->      
        editView ctx club user None article published []


let createPost (club: Club) (user: Users.User) (ctx: HttpContext) =

    let db = ctx.Database
 
    let form = ctx.BindForm<ArticleModel>()

    printf "\n --------------- \n  %O \n"   { form with 
                                                       IsMatchReport = (string ctx.Request.Form.["IsMatchReport"]) = "on" // Workaround
    }

    { form with 
        IsMatchReport = (string ctx.Request.Form.["IsMatchReport"]) = "on" // Workaround
    } ==>
        [        
           <@ form.Headline @> >- [isRequired]
           <@ form.Content @> >- [isRequired]
        ]
        |> function
        | Ok form -> 
            Persistence.createArticle db club.Id user form
            |> Results.bind 
                (fun name -> Redirect <| sprintf "/nyheter/vis/%s" name)
            

        | Error validationErrors ->          
                (form, System.DateTime.Now)
                |> fun (article, published) ->
                    editView ctx club user None article published validationErrors

let delete (club: Club) name (ctx: HttpContext) =
    
    let db = ctx.Database
        
    Persistence.deleteArticle db club.Id name
    |> Results.bind (fun _ -> Redirect "/")
            

      
