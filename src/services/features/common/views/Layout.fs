namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open Giraffe.GiraffeViewEngine.Attributes
open MyTeam
open MyTeam.Domain
open ExpressionOptimizer
open System
open System.Linq

module Analytics = 

    let trackingId = "UA-69971219-1"
    let script = sprintf "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
          (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
          m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
          })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

          ga('create', '%s', 'auto');
          ga('send', 'pageview');" trackingId
          

module Images =
    let get string = 
        "http://res.cloudinary.com/dvtgbryrv/image/upload/c_scale,w_100,q_100/v1462117763/wamkam_med_vfkizt.png"

    let getRz string number = 
        sprintf "http://res.cloudinary.com/dvtgbryrv/image/upload/c_scale,w_%i,q_100/v1462117763/wamkam_med_vfkizt.png" number


    let getMember url facebookId width height =
        "url"

[<AutoOpen>]
module SharedStuffs =

    let fa name = 
        sprintf "fa fa-%s" name

    let icon name =
        i [_class name ] []

    let whitespace = rawText "&nbsp;"

module Icon = 
    let attendance = icon <| fa "check-square-o"
    let coach = icon <| "flaticon-football50"
    let signup = icon <| fa "calendar"
    let previous = icon <| fa "history"
    let upcoming = icon <| fa "calendar-o"
    let squadList = icon <| fa "users"
    let fine = icon <| fa "money"
    let news = icon <| fa "newspaper"
    let payment = icon <| fa "list"
    let assist = icon <| "flaticon-football119"
    let goal = icon <| fa "soccer-ball-o"
    let player = icon <| "flaticon-soccer18"
    let training = icon <| "flaticon-couple40"
    let user = icon <| fa "user"
    let yellowCard = icon <| "icon icon-card-yellow"
    let redCard = icon <| "icon icon-card-red"
        

[<AutoOpen>]
module Pages =   

    let (=??) (first: string) (second: string) =
        if first.HasValue then first else second           


    let (=?) (condition: bool) (first, second) =
        if condition then first else second   

    let coachMenuItems = 
            [
                li [_href "/intern/arrangement/ny"] [Icon.training; encodedText "Opprett arrangement"]
                li [_href "/intern/admin"] [Icon.user; encodedText "Administrer spillere"]
                li [_href "/intern/admin/spillerinvitasjon"] [icon <| fa "plus"; encodedText "Legg til spiller"]
                li [_href "/intern/nyheter/ny"] [Icon.news; encodedText "Skriv artikkel"]
            ]


    type NotificationButtonModel = {
        UnansweredEventIds: Guid list
    } with 
        member m.UnansweredEvents = m.UnansweredEventIds.Length


    let notifications (ctx: HttpContext) club (user: Users.User) =
        let (ClubId clubId) = club.Id
        let db = ctx.Database
        let now = DateTime.Now
        let inFourDays = now.AddDays(4.0)
          
        let getEventsAndAttendances () =
                
            let events = query  
                                { 
                                    for e in db.Events do
                                    where (e.ClubId = clubId 
                                                && e.DateTime < inFourDays 
                                                && (e.DateTime > now) 
                                                && not e.IsPublished) 
                                    join eventTeam in db.EventTeams on (e.Id = eventTeam.EventId)                  
                                    select (e.Id, eventTeam.TeamId)
                                } |> Seq.toList
                                  |> List.distinct                       


            let eventIds =
                let eventIds = events |> List.map(fun (id, _) -> id) |> List.distinct
                query {
                    for e in eventIds do
                    select e
                }

            let attendances = 
                query {
                    for attendance in db.EventAttendances do
                    where (eventIds.Contains(attendance.EventId))
                    select (attendance.EventId, attendance.MemberId)
                }
                |> Seq.toList

            (events, attendances)
        
        
        let (events, attendances) =         
            let key = sprintf "notifications-%O" clubId
            Cache.get ctx key getEventsAndAttendances


        let userEvents = events
                          |> List.filter (fun (_, teamId) -> user.TeamIds |> List.contains teamId)
                          |> List.map (fun (eventId, _) -> eventId)
                          |> List.distinct                       


        let userAttendances =
            attendances 
            |> List.filter (fun (_, memberId) -> memberId = user.Id)
            |> List.map (fun (eventId, _) -> eventId)

        let unansweredEventIds = 
            userEvents
            |> List.filter (fun (eventId) -> not (userAttendances |> List.contains eventId))
            |> List.distinct            
      

        let model = {
            UnansweredEventIds = unansweredEventIds
        }

        ul [_id "notification-button";_class "notification-button nav navbar-nav navbar-right navbar-topRight--item"] [
            (if model.UnansweredEvents > 0 then            
                li [_class "dropdown" ] [ 
                    button [_class "dropdown-toggle btn btn-warning"; attr "data-toggle" "dropdown" ] [
                        icon <| fa "bell-o"
                    ]
                    ul [_class "dropdown-menu" ] [
                        li [] [
                            a [_href <| sprintf "/intern#event-%O" (model.UnansweredEventIds |> Seq.head) ] [
                                Icon.signup
                                whitespace
                                whitespace
                                span [_class "hidden-xxs" ] [ 
                                    encodedText "Du har "
                                ]
                                encodedText <| sprintf "%i %s" model.UnansweredEvents (model.UnansweredEvents > 1 =? (" ubesvarte arrangementer", " ubesvart arrangement"))
                            ]
                        ]                    
                    ]
                ]
             else emptyText)
        ]
           
    


    let loginPartial ctx club user  =
        let notifications = notifications ctx
        user |> Option.fold (fun _ (user: Users.User) ->
                                        span [] [
                                            div [_class "login-image-wrapper"] [
                                                a [_title user.Name; _href <| "/spillere/vis/" + user.UrlName ] [
                                                    img [_src <| Images.getMember user.Image user.FacebookId 40 40 ]
                                                ]
                                            ]
                                            notifications club user 
                                        ]                                      
                            ) emptyText

    let alerts = emptyText                    




    type LayoutModel = {
        Title: string
        PageName: string
        MetaDescription: string
        Scripts: XmlNode list
    }
 
    let layout club (user: Option<Users.User>) getOptions (ctx: HttpContext) content =  
        let o = getOptions ({
                                Title = ""
                                PageName = ""
                                MetaDescription = ""
                                Scripts = []
                            })

        let loginPartial = loginPartial ctx club                       

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
            body []([
                    div [_class "navbar navbar-inverse navbar-fixed-top"] [
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
                                    li [] [a [_href "/statistikk" ] [ encodedText "Statistikk"] ]
                                    li [] [a [_href "/om" ] [ encodedText "Om klubben"] ]
                                    user |> Option.fold(fun _ _ -> 
                                                            li [] [
                                                                a [_class "slide-down-parent hidden-xs"; attr "data-submenu" "#submenu-internal"; _href "javascript:void(0)" ] [
                                                                    encodedText "Intern "
                                                                    icon <| fa "angle-down"
                                                                ] 
                                                            ]
                                                                                                
                                                        ) (emptyText)
                     

                          
                                ]
                                user |> Option.fold (fun _ user ->
                                            div [] [
                                                hr [_class "visible-xs submenu-divider" ]
                                                ul [_class "nav navbar-nav submenu visible-xs"] [
                                                    li [] [a [_href "/intern" ] [Icon.signup; encodedText " Intern"]]
                                                    li [] [a [_href "/intern/oppmote" ] [Icon.attendance; encodedText " Oppmøte"]]
                                                    li [] [a [_href "/intern/boter" ] [Icon.fine; encodedText " Bøter"]]
                                                    li [] [a [_href "/intern/lagliste" ] [Icon.squadList; encodedText " Lagliste"]]                       
                                                ]                                        
                                                user.IsInRole [Role.Admin;Role.Trener] =?
                                                    (div [] [
                                                        hr [_class "visible-xs submenu-divider"]
                                                        ul [_class "nav navbar-nav submenu visible-xs adminMenu"] 
                                                            coachMenuItems
                                                        ], emptyText)
                                            ]

                                ) emptyText
                        
                            ]
                            div [_class "navbar-topRight"] [
                                loginPartial user
                            ]
                            user |> Option.fold (fun _ user ->
                                                    ul [_id "submenu-internal";_class "nav navbar-nav submenu slide-down-child hidden-xs collapse" ] [
                                                            li [] [a [_href "/intern" ] [Icon.signup; encodedText " Intern"]]
                                                            li [] [a [_href "/intern/oppmote" ] [Icon.attendance; encodedText " Oppmøte"]]
                                                            li [] [a [_href "/intern/boter" ] [Icon.fine; encodedText " Bøter"]]
                                                            li [] [a [_href "/intern/lagliste" ] [Icon.squadList; encodedText " Lagliste"]]                       
                                                            user.IsInRole [Role.Admin;Role.Trener] =?
                                                                (li [] [a [_href "/intern/admin" ] [Icon.coach; encodedText " Admin"]]                       
                                                                , emptyText)
                                                                ]

                                ) emptyText                               
                        ]
                    ]
                    div [_class "mt-page-header"] [
                        div [_class "container"] [
                            h1 [] [
                                encodedText (o.Title =?? o.PageName)
                            ]                           
                        ]
                    ]
                    div [_id "main-container";_class "container"] 
                        (List.append [ 
                            div [_id "alerts";_class "col-lg-9 col-md-9"][
                                alerts
                            ]
                            div [_class "clearfix" ] []
                        ] 
                        content)

                    script [_src "/compiled/lib/lib.bundle.js?v1"] []
                    script [_src "/compiled/app.js?v15" ] []
            ] |> List.append o.Scripts)                
        ]          
    
         
  