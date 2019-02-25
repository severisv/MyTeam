module MyTeam.News.Pages.Index

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.News
open MyTeam.News.Pages
open MyTeam.Views
open Server.Common.News
open MyTeam.Views.BaseComponents


let private pagination articles =   

    let page = articles.PaginationOptions

    let previousUrl = sprintf "/%i/%i" (page.Skip - page.Take) page.Take
    let nextUrl = sprintf "/%i/%i" (page.Skip + page.Take) page.Take

    div [ _class "mt-container" ] [ 
        nav [ _class "news-pager" ] [ 
            ul [ _class "pager" ] [ 
                li [ _class <| (articles.HasPrevious =? ("", "disabled")) ][
                    a [ (articles.HasPrevious =? (_href previousUrl, _empty)) ] [ encodedText "← Nyere" ]
                ]
                li [ _class <| (articles.HasNext =? ("", "disabled")) ][
                    a [ (articles.HasNext =? (_href nextUrl, _empty))] [ encodedText "Eldre →" ]
                ]                          
            ]
        ]
    ]           

let view (club: Club) (user: User option) getPaginationOptions  (ctx: HttpContext) =

    let paginationOptions = getPaginationOptions { Skip = 0; Take = 6 }

    let db = ctx.Database
    let description = Queries.getClubDescription db club.Id

    let articles = Queries.listArticles db club.Id paginationOptions

    [
        mtMain [] 
            ((articles.Items |> List.map (fun article -> 
                let articleUrl = Components.articleUrl article

                block [ _class "news-item"] [ 
                    div [ _class "news-imageWrapper news-imageWrapper--short" ] [
                        a [ _href <| articleUrl ] [ 
                            img [ _src <| Components.image ctx article.Image ] ]
                    ]
                    Components.editLink article user
                    h2 [] [ 
                        a [ _class "news-headline"; _href <| articleUrl ]
                          [ encodedText article.Headline ] ]
                    hr [ _class "sm" ]
                    a [ _class "underline"; _href <| articleUrl ][ 
                        encodedText "Les hele" ]
                 
                ]                      

            )) @ 
            [ 
                pagination articles     
            ])
        sidebar [] [
            Components.adminMenu user
            Components.articleNav db club
            Components.twitterFeed
        ]    
    ]
    |> layout club user (fun o -> { o with 
                                        Title = "Nyheter"
                                        MetaDescription = description |> Option.defaultValue "" 
                                        Scripts = [ Components.twitterScript ]
                                    }
                        ) ctx
    |> OkResult
