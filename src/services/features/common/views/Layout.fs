namespace MyTeam

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Views


[<AutoOpen>]
module Pages =   

    let coachMenuItems = 
            [
                li [_href "/intern/arrangement/ny"] [Icons.training ""; encodedText "Opprett arrangement"]
                li [_href "/intern/admin"] [Icons.user ""; encodedText "Administrer spillere"]
                li [_href "/intern/admin/spillerinvitasjon"] [icon <| fa "plus" <| ""; encodedText "Legg til spiller"]
                li [_href "/intern/nyheter/ny"] [Icons.news ""; encodedText "Skriv artikkel"]
            ]

 
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

        let notifications = notifications ctx club
        let loginPartial = userPartial ctx notifications        
        let getImage = Images.get ctx                

        html [_lang "nb-no"] [
            head [] [
                meta [_charset "utf-8"]
                meta [_name "viewport";_content "width device-width, initial-scale 1.0"]
                meta [_title <| club.ShortName + (o.Title.HasValue =? (" - " + o.Title, "")) ]
                o.MetaDescription.HasValue =? (meta [_name "description"; _content o.MetaDescription], emptyText)
                title [] [encodedText <| club.Name + (o.Title.HasValue =? (" - " + o.Title, "")) ]
                link [_rel "icon"; _type "image/png"; _href <| getImage (club.Favicon =?? club.Logo) id]
                link [_rel "stylesheet"; _href "/compiled/site.bundle.css" ]
                Analytics.script
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
                                a [_href "/";_class "navbar-brand" ] [ img [_src <| getImage club.Logo (fun o -> { o with Width = Some 100 }); _alt club.ShortName ] ]
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
                                                                    icon <| fa "angle-down" <| ""
                                                                ] 
                                                            ]
                                                                                                
                                                        ) (emptyText)                     
                          
                                ]
                                user |> Option.fold (fun _ user ->
                                            div [] [
                                                hr [_class "visible-xs submenu-divider" ]
                                                ul [_class "nav navbar-nav submenu visible-xs"] [
                                                    li [] [a [_href "/intern" ] [Icons.signup ""; encodedText " Intern"]]
                                                    li [] [a [_href "/intern/oppmote" ] [Icons.attendance ""; encodedText " Oppmøte"]]
                                                    li [] [a [_href "/intern/boter" ] [Icons.fine ""; encodedText " Bøter"]]
                                                    li [] [a [_href "/intern/lagliste" ] [Icons.squadList ""; encodedText " Lagliste"]]                       
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
                            loginPartial user                            
                            user |> Option.fold (fun _ user ->
                                                    ul [_id "submenu-internal";_class "nav navbar-nav submenu slide-down-child hidden-xs collapse" ] [
                                                            li [] [a [_href "/intern" ] [Icons.signup ""; encodedText " Intern"]]
                                                            li [] [a [_href "/intern/oppmote" ] [Icons.attendance ""; encodedText " Oppmøte"]]
                                                            li [] [a [_href "/intern/boter" ] [Icons.fine ""; encodedText " Bøter"]]
                                                            li [] [a [_href "/intern/lagliste" ] [Icons.squadList ""; encodedText " Lagliste"]]                       
                                                            user.IsInRole [Role.Admin;Role.Trener] =?
                                                                (li [] [a [_href "/intern/admin" ] [Icons.coach ""; encodedText " Admin"]]                       
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
    
         
  