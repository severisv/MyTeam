namespace MyTeam.Views

open Giraffe.GiraffeViewEngine

module Analytics = 

    let trackingId = "UA-69971219-1"
    let script =   
            script [] [ rawText <| sprintf "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
                          (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
                          m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
                          })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

                          ga('create', '%s', 'auto');
                          ga('send', 'pageview');" trackingId 
                    ]
         
          

      
