module MyTeam.Views.FacebookSdk

open MyTeam
open Giraffe
open Giraffe.GiraffeViewEngine
open Microsoft.Extensions.Options

let script (ctx: HttpContext) = 
    let appId = ctx.GetService<IOptions<FacebookOptions>>().Value.AppId
    script [] [
        rawText <| sprintf "window.fbAsyncInit = function() {
                    FB.init({
                      appId      : '%s',
                      xfbml      : true,
                      version    : 'v2.5'
                    });

                    window.mt_fb.isLoaded = true;
                  };

                  (function (d, s, id) {

                     var js, fjs = d.getElementsByTagName(s)[0];
                     if (d.getElementById(id)) {return;}
                     js = d.createElement(s); js.id = id;
                     js.src = '//connect.facebook.net/en_US/sdk.js';
                     fjs.parentNode.insertBefore(js, fjs);
                   }(document, 'script', 'facebook-jssdk'));" appId
]

