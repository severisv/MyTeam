module MyTeam.News.Pages.Components

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.News
open System
open MyTeam.Views
open MyTeam.Shared.Components

let articleUrl (article: Article) = 
    sprintf "/nyheter/vis/%O" article.Name


let editLink (article: Article) (user: Users.User option) =
    user |> Option.bind (fun user ->
                            if user.IsInRole [Role.Admin;Role.Skribent;Role.Trener] then
                                Some <| a [ _class "pull-right edit-link"; _href <| sprintf "/nyheter/endre?navn=%s" article.Name ] [ 
                                           !!(Icons.edit "Rediger artikkel")
                                ]
                            else None
                        )
    |> Option.defaultValue empty
    

let matchDetails = 
        div [] [
            h2 [ _class "news-matchReport" ] [ encodedText "Kamprapport" ]
            hr [ ]
        ]
   

let image ctx article = 
    Images.getArticle ctx article.Image (fun o -> { o with Format = Some Jpg; Quality = 85; Width = Some 1280  })


let adminMenu (user: Users.User option) =
    user |> Option.bind(fun user ->
        if user.IsInRole [Role.Admin;Role.Skribent;Role.Trener] then
            Some <| block [] [ 
                        ul [ _class "nav nav-list" ] [ 
                            li [ _class "nav-header" ] [ 
                                encodedText "Adminmeny" ]
                            li [][ 
                                a [ _href "/nyheter/ny" ][ 
                                    i [ _class "fa fa-plus" ] [] 
                                    encodedText " Skriv ny artikkel" 
                                ]
                            ]
                    ]
                ]
        else None
    )
    |> Option.defaultValue empty
  

let articleNav articles =

    block [] [
        div [ _class "articleNav "] 
            (articles 
            |> List.groupBy (fun a -> sprintf "%s %i" (a.Published.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture)) a.Published.Year)
            |> List.map (fun (key, values) ->
                                ul [ _class "nav nav-list" ]
                                    ([
                                        li [ _class "nav-header" ] [ encodedText <| string key]
                                    ] @
                                    (values |> List.map (fun article ->
                                                             li [] [ 
                                                                a [ _href <| articleUrl article ] [ 
                                                                    encodedText <| truncate 22 article.Headline ]
                                                                ]                                           
                                    ))
                                    )       
            ))
    ]
   

let twitterFeed =
    block [] [
        h4 [] [ 
           i [ _class "fa fa-twitter" ] [] 
           encodedText "Siste Tweets" 
        ]
        a [ 
            _class "twitter-timeline" 
            _href "https://twitter.com/WamKam"
            attr "data-widget-id" "672159319203430401"
            attr "data-tweet-limit" "4" 
            attr "data-chrome" "nofooter transparent noheader noborders noscrollbar" ] [ 
                encodedText "Tweets av WamKam" 
        ]
    ]

let twitterScript =
    script [] [
        rawText "!function (d, s, id) {\
                var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ?\
                'http' : 'https'; if (!d.getElementById(id)) {\
                    js = d.createElement(s); js.id = id; js.src = p + '://platform.twitter.com/widgets.js';\
                    fjs.parentNode.insertBefore(js, fjs);\
                }\
                }(document, 'script', 'twitter-wjs');"
    
    ]




