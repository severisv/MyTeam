module MyTeam.News.Pages.Components

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain
open Shared.Domain.Members
open MyTeam.News
open MyTeam.Views
open Server.Common.News
open MyTeam.Views.BaseComponents

let articleUrl (article: Article) = sprintf "/nyheter/vis/%O" article.Name

let adminMenu (user: User option) =
    user
    |> Option.bind (fun user ->
        if
            user.IsInRole [
                Role.Admin
                Role.Skribent
                Role.Trener
            ] then
            Some
            <| block [] [
                ul [ _class "nav nav-list" ] [
                    li [ _class "nav-header" ] [
                        encodedText "Admin"
                    ]
                    li [] [
                        a [ _href "/nyheter/ny" ] [
                            i [ _class "fa fa-plus" ] []
                            encodedText " Skriv ny artikkel"
                        ]
                    ]
                ]
               ]
        else
            None)
    |> Option.defaultValue empty

let articleNav db (club: Club) =
    let articles =
        Queries.listArticles db club.Id { Skip = 0; Take = 10 }
        |> fun a -> a.Items

    block [] [
        div
            [ _class "articleNav " ]
            (articles
             |> List.groupBy (fun a ->
                 sprintf "%s %i" (a.Published.ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture)) a.Published.Year)
             |> List.map (fun (key, values) ->
                 ul
                     [ _class "nav nav-list" ]
                     ([ li [ _class "nav-header" ] [
                            encodedText <| string key
                        ] ]
                      @ (values
                         |> List.map (fun article ->
                             li [] [
                                 a [ _href <| articleUrl article ] [
                                     encodedText <| truncate 22 article.Headline
                                 ]
                             ])))))
    ]


let twitterFeed = div [] []
    // block [_style "padding-left: 0; padding-right: 0;"] [
    //     h4 [] [
    //         i [ _class "fa fa-twitter" ] []
    //         whitespace
    //         encodedText "Siste Tweets"
    //     ]
    //     a [ 
    //         _class "twitter-timeline"
    //         _href """https://twitter.com/WamKam?ref_src=twsrc%5Etfw"""
    //         attr "data-chrome" "nofooter transparent noheader noborders noscrollbar"

    //         ] [
    //         encodedText "Tweets av WamKam"
    //     ]
    // ]



let twitterScript =
    script [
                _src "https://platform.twitter.com/widgets.js"
                _charset "utf-8"
                _async ] []



let tinyMceScripts =
    let elementId = ".tinymce"

    [ script [ _type "text/javascript"
               _src "/compiled/lib/tinymce/tinymce.min.js" ] []
      script [ _type "text/javascript" ] [
          rawText
          <| sprintf
              "tinymce.init({
                height: '480',
                selector: '%s',
                plugins: [
                    'image',
                    'link',
                    'media',
                    'code',
                    'lists'
                ],
                toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media | ',
            });"
              elementId
      ] ]
