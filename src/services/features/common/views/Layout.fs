namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Giraffe.GiraffeViewEngine.Attributes
open MyTeam
open MyTeam.Domain

module Analytics = 

    let id = "UA-69971219-1"
    let script = sprintf "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
          (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
          m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
          })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

          ga('create', '%s', 'auto');
          ga('send', 'pageview');" id
          

module Images =
    let get string = 
        "url"

    let getRz string number = 
        "url"    


[<AutoOpen>]
module SharedStuffs =
    let icon name =
        i [_class <| sprintf "fa fa-%s" name ] []
        

[<AutoOpen>]
module Pages =   

    let (=??) (first: string) (second: string) =
        if first.HasValue then first else second           


    let (=?) (condition: bool) (first, second) =
        if condition then first else second   


    type ViewOptions = {
        Title: string
        MetaDescription: string
    }
 
    let layout club (user: Option<Users.User>) getOptions (content: XmlNode list) =  
        let o = getOptions ({
                                Title = ""
                                MetaDescription = ""
                            })

        html [_lang "nb-no"] [
            head [] [
                meta [_charset "utf-8"]
                meta [_name "viewport";_content "width device-width, initial-scale 1.0"]
                meta [_title <| club.ShortName + (o.Title.HasValue =? (" - " + o.Title, "")) ]
                o.MetaDescription.HasValue =? (meta [_name "description"; _content o.MetaDescription], emptyText)
                title [] [encodedText <| club.Name + (o.Title.HasValue =? (" - " + o.Title, "")) ]
                link [_rel "icon"; _type "image/png"; _href <| Images.get (club.Favicon =?? club.Logo)]
                link [_rel "stylesheet"; _href "/compiled/site.bundle.css?v16" ]
                script [] [ rawText Analytics.script ]
            ]
            body [] [
                    div [_class "navbar navbar-inverse navbar-fixed-top"] []
                    div [_class "container" ] [
                        div [_class "navbar-header"] [
                            button [_type "button";_class "navbar-toggle"; attr "data-toggle" "collapse"; attr "data-target" ".navbar-collapse"] [
                                span [_class "icon-bar" ] []
                                span [_class "icon-bar" ] []
                                span [_class "icon-bar" ] []
                            ]
                            a [_class "navbar-brand" ] [ img [_src <| Images.getRz club.Logo 100; _alt club.ShortName ] ]
                        ]
                        div [_class "navbar-collapse collapse" ] [
                            ul [_class "nav navbar-nav"] [
                                li [] [a [_href "/spillere" ] [ encodedText "Spillere"] ]
                                li [] [a [_href "/kamper" ] [ encodedText "Kamper"] ]
                                li [] [a [_href "/tabell" ] [ encodedText "Tabell"] ]
                                li [] [a [_href "/statistikk" ] [ encodedText "statistikk"] ]
                                li [] [a [_href "/om" ] [ encodedText "Om klubben"] ]
                                user |> Option.fold(fun _ _ -> 
                                                        li [] [
                                                            a [_class "slide-down-parent hidden-xs"; attr "data-submenu" "#submenu-internal"; _href "javascript:void(0)" ] [
                                                                encodedText "Intern "
                                                                icon "angle-down"
                                                            ] 
                                                        ]
                                                                                            
                                                    ) (emptyText)

                      
                            ]
                        ]

                    ]
            ]
                
        ]            
    
    
                
         
   
            //         @if (Context.UserIsMember())
            //         {
            //             hr class "visible-xs submenu-divider" /[]
            //             ul class "nav navbar-nav submenu visible-xs"[]
            //                 li[]a asp-controller "event" asp-action "index"[]i class "@Icons.Signup"[]/i[] @Res.Signup/a[]/li[]
            //                 li[]a asp-controller "attendance" asp-action "attendance"[]i class "@Icons.Attendance"[]/i[] @Res.Attendance/a[]/li[]
            //                 li[]a asp-controller "fine" asp-action "index"[]i class "@Icons.Fine"[]/i[] Bøter/a[]/li[]
            //                 li[]a asp-controller "member" asp-action "index"[]i class "@Icons.SquadList"[]/i[] @Res.SquadList/a[]/li[]
            //             /ul[]
            //             if (Context.UserIsInRole(Roles.Admin, Roles.Coach))
            //                 {
            //                 hr class "visible-xs submenu-divider" /[]
            //                 ul class "nav navbar-nav submenu visible-xs adminMenu"[]
            //                     @Html.Partial("_CoachMenuItems")
            //                 /ul[]
            //                 }
            //         }
            //         /div[]
            //         div class "navbar-topRight"[]
            //             @await Html.PartialAsync("_LoginPartial")
            //             @if (Context.UserIsMember())
            //             {
            //                 @Html.Partial("_UserInfo", Context.Member())
            //                 ul id "notification-button" class "notification-button nav navbar-nav navbar-right navbar-topRight--item"[]
            //                     @await Component.InvokeAsync("NotificationButton")
            //               /ul[]
            //                     }
            // /div[]

            //         @if (Context.UserIsMember())
            //         {
            //             ul id "submenu-internal" class "nav navbar-nav submenu slide-down-child  hidden-xs collapse"[]
            //                 li[]a asp-controller "event" asp-action "index"[]i class "@Icons.Signup"[]/i[] @Res.Signup/a[]/li[]
            //                 li[]a asp-controller "attendance" asp-action "attendance"[]i class "@Icons.Attendance"[]/i[] @Res.Attendance/a[]/li[]
            //                 li[]a asp-controller "fine" asp-action "index"[]i class "@Icons.Fine"[]/i[] Bøter/a[]/li[]
            //                 li[]a asp-controller "member" asp-action "index"[]i class "@Icons.SquadList"[]/i[] @Res.SquadList/a[]/li[]
            //                 @if (Context.UserIsInRole(Roles.Admin, Roles.Coach))
            //                 {
            //                     li[]a asp-controller "admin" asp-action "index"[]i class "@Icons.Coach"[]/i[] @Res.AdminPage/a[]/li[]
            //                 }
            //             /ul[]
            //         }
            //     /div[]
            // /div[]
            // @await Component.InvokeAsync("PageHeader", new { headline   ViewBag.PageName ?? ViewBag.Title ?? "", content   ViewBag.Subtext ?? "" })
            // div id "main-container" class "container"[]
            //     div id "alerts" class "col-lg-9 col-md-9"[]
            //         @Html.Partial("_Alerts")
            //     /div[]
            //     div class "clearfix"[]/div[]
            //     @RenderBody()
            //     footer[]/footer[]
            // /div[]
            // script src "~/compiled/lib/lib.bundle.js?v1"[]/script[]
            // script src "~/compiled/app.js?v15"[]/script[]
            // @RenderSection("scripts", required: false)
            // script[]console.log('Server side: @Context.Elapsed()ms');/script[]
            // /body[]
