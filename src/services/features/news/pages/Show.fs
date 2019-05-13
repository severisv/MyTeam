module MyTeam.News.Pages.Show

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open MyTeam.Views
open Shared.Domain
open Shared.Domain.Members
open MyTeam.News
open MyTeam.News.Pages
open Server.Common.News
open Server

let view (club: Club) (user: User option) name (ctx: HttpContext) =


    let db = ctx.Database

    Queries.getArticle db club.Id name
    |> function
    | None -> NotFound
    | Some article ->    
        [
            mtMain [] [  
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
                                            MetaTitle = article.Details.Headline
                                            Title = "Nyheter"
                                            Scripts = [ Components.twitterScript ]
                                        }
                            ) ctx
        |> OkResult
