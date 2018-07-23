module MyTeam.News.Pages.Show

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.News
open MyTeam.News.Pages
open MyTeam.Views



let view (club: Club) (user: Users.User option) name (ctx: HttpContext) =


    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
    | None -> NotFound
    | Some article ->    

        [
            main [] [       
                    block [ _class "news-item"] [ 
                        div [ _class "news-imageWrapper" ] [
                            img [ _src <| Components.image ctx article.Details.Image ] ]                    
                        Components.editLink article.Details user
                        h2 [] [ 
                            encodedText article.Details.Headline ]
                        
                        p [ _class "news-author" ] [ 
                            a [ _class "underline"; _href <| sprintf "/spillere/vis/%s" article.Author.UrlName ] [ 
                                encodedText article.Author.Name 
                            ]          
                            span [ _class "datetime" ] [ encodedText <| sprintf " %s" (Date.format article.Details.Published) ]            
                            article.GameId 
                            |> Option.map (fun gameId ->
                                  a [ 
                                      _href <| sprintf "/kamper/vis/%O" gameId
                                      _class "pull-right u-font-normal" 
                                    ]
                                    [ 
                                        i [ _class "fa fa-info-circle" ] [] 
                                        encodedText " Kampdetaljer" 
                                    ])
                            |> Option.defaultValue empty                      
                        ]
                        hr [ _class "sm" ]
                        div [ _class "news-content" ][ rawText article.Content ]
                    ]                      
            ]          
            sidebar [] [
                Components.articleNav db club
                Components.twitterFeed
            ]    
        ]
        |> layout club user (fun o -> { o with 
                                            Title = "Nyheter"
                                            Scripts = [ Components.twitterScript ]
                                        }
                            ) ctx
        |> OkResult
