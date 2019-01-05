module MyTeam.News.Pages.Components

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.News
open System
open MyTeam.Views
open MyTeam.Common.News

let articleUrl (article: Article) = 
    sprintf "/nyheter/vis/%O" article.Name



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
  

let articleNav db (club: Club) =
    let articles = 
        Queries.listArticles db club.Id { Skip = 0; Take = 10 }
        |> fun a -> a.Items

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
           whitespace
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



let tinyMceScripts = 
    let elementId = ".tinymce"
    [
       script [ _type "text/javascript"; _src "/compiled/lib/tinymce/tinymce.min.js" ] []
       script [ _type "text/javascript" ] [ 
           rawText <| sprintf "tinymce.init({
                selector: '%s',
                theme: 'modern',
                plugins: [
                    'advlist autolink link image lists charmap hr anchor pagebreak',
                    'wordcount visualblocks visualchars insertdatetime media nonbreaking',
                    'save table contextmenu directionality template paste code codesample'
                ],
                toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | media | ',
                extended_valid_elements: 'script[language|type|src|async]',
                convert_urls: false
            });" elementId ]
    ]



let cloudinaryScripts (options: CloudinarySettings) =

    let unixTimestamp = int (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds

    let queryString = sprintf "timestamp=%i%s" unixTimestamp options.ApiSecret
    let signature = Sha1.hashStringForUTF8String queryString

    [
        script [_src "/compiled/lib/cloudinary.bundle.js" ][]
        script [] [

            rawText 
                <| sprintf "$.cloudinary.config({ cloud_name: '%s', api_key: '%s', signature: '%s' });

                                    var formData = {
                                        api_key: '%s',
                                        timestamp: '%i',
                                        signature: '%s'
                                    }

                                    $('.cloudinary-fileupload').each(function () {
                                        $(this).attr('data-form-data', JSON.stringify(formData));
                                    });

                                    $('.cloudinary-fileupload').bind('cloudinarydone', function (e, data) {
                                        $('.cloudinary-preview').html(
                                          $.cloudinary.image(data.result.public_id,
                                            {
                                                format: data.result.format, version: data.result.version,
                                                crop: 'fill'
                                            })
                                        );
                                        return true;

                                    });

                                    $('.cloudinary-fileupload').bind('fileuploadprogress', function (e, data) {
                                        $('.cloudinary-progress-bar').css('width', Math.round((data.loaded * 100.0) / data.total) + '%%');
                                    });" options.CloudName options.ApiKey signature options.ApiKey unixTimestamp signature
                                    
                                 ]      
    ]