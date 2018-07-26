module MyTeam.News.Pages.Show

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Views
open MyTeam.Domain
open MyTeam.News
open MyTeam.News.Pages
open MyTeam.Common.News

let view (club: Club) (user: Users.User option) name (ctx: HttpContext) =


    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
    | None -> NotFound
    | Some article ->    
        [
            main [] [  
                block [] [     
                    Common.News.Components.showArticle ctx user 
                        article
                        (article.GameId 
                          |> Option.map (fun gameId ->
                                a [ 
                                    _href <| sprintf "/kamper/vis/%O" gameId
                                    _class "pull-right u-font-normal" 
                                  ]
                                  [ 
                                      i [ _class "fa fa-info-circle" ] [] 
                                      encodedText " Kampdetaljer" 
                                  ]))
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
