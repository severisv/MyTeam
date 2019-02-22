namespace MyTeam

open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain.Members
open Shared.Components
open Microsoft.Extensions.Hosting
open MyTeam.Views
open MyTeam.Views.BaseComponents

[<AutoOpen>]
module Pages =

    let coachMenuItems = Views.Admin.coachMenuItems


    let internalMenuItems =
        [   li [] [ a [ _href "/intern" ] [ !!(Icons.signup ""); encodedText " Påmelding" ] ]
            li [] [ a [ _href "/intern/oppmote" ] [ !!(Icons.attendance ""); encodedText " Oppmøte" ] ]
            li [] [ a [ _href "/intern/boter" ] [ !!(Icons.fine ""); encodedText " Bøter" ] ]
            li [] [ a [ _href "/intern/lagliste" ] [ !!(Icons.squadList ""); encodedText " Lagliste" ] ] ]

    type LayoutModel = {
        Title : string
        MetaTitle : string
        MetaDescription : string
        Scripts : XmlNode list
    }

    let layout club (user : Option<Users.User>) getOptions (ctx : HttpContext) content =

        let isProduction = ctx.GetService<IHostingEnvironment>().IsProduction()

        let o = getOptions ({ Title = ""
                              MetaTitle = ""
                              MetaDescription = ""
                              Scripts = [] })

        let notifications = notifications ctx club
        let loginPartial = userPartial ctx notifications
        let getImage = Images.get ctx

        html [ _lang "nb-no" ] [
            head [] [
                meta [ _charset "utf-8" ]
                meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
                meta [ _title <| club.ShortName + (Strings.hasValue o.Title =? (" - " + o.Title, "")) ]
                Strings.hasValue o.MetaDescription =? (meta [ _name "description"; _content o.MetaDescription ], emptyText)
                title [] [ encodedText (o.MetaTitle =?? (sprintf "%s - %s" club.Name o.Title)) ]
                link [ _rel "icon"; _type "image/png"; _href <| getImage id (club.Favicon =?? club.Logo) ]
                link [ _rel "apple-touch-icon"; _type "image/png"; _href <| getImage id (club.Favicon =?? club.Logo) ]
                link [ _rel "stylesheet"; _href "/compiled/site.bundle.css?v7" ]
                isProduction =? (Analytics.script, empty)
            ]
            body [] ([
                    div [ _class "navbar navbar-inverse navbar-fixed-top" ] [
                        div [ _class "container" ] [
                            div [ _class "navbar-header" ] [
                                button [ _type "button"; _class "navbar-toggle"; attr "data-toggle" "collapse"; attr "data-target" ".navbar-collapse" ] [
                                    span [ _class "icon-bar" ] []
                                    span [ _class "icon-bar" ] []
                                    span [ _class "icon-bar" ] []
                                ]
                                a [ _href "/"; _class "navbar-brand" ] [ img [ _src <| getImage (fun o -> { o with Width = Some 100 }) club.Logo; _alt club.ShortName ] ]
                            ]
                            div [ _class "navbar-collapse collapse" ]
                                ([
                                    ul [ _class "nav navbar-nav" ] [
                                        li [] [ a [ _href "/spillere" ] [ encodedText "Spillere" ] ]
                                        li [] [ a [ _href "/kamper" ] [ encodedText "Kamper" ] ]
                                        li [] [ a [ _href "/tabell" ] [ encodedText "Tabell" ] ]
                                        li [] [ a [ _href "/statistikk" ] [ encodedText "Statistikk" ] ]
                                        li [] [ a [ _href "/om" ] [ encodedText "Om klubben" ] ]
                                        li [ _class "hidden-sm" ] [ a [ _href "/støttespillere" ] [ encodedText "Støttespillere" ] ]
                                        user |> Option.fold (fun _ _ ->
                                                                li [] [
                                                                    a [ _class "slide-down-parent hidden-xs"; attr "data-submenu" "#submenu-internal"; _href "javascript:void(0)" ] [
                                                                        encodedText "Intern "
                                                                        icon <| fa "angle-down" <| ""
                                                                    ]
                                                                ]

                                                            ) (emptyText)
                                    ]
                                ] @ (user |> Option.fold (fun _ user ->
                                            ([
                                                    hr [ _class "visible-xs submenu-divider" ]
                                                    ul [ _class "nav navbar-nav submenu visible-xs" ]
                                                        internalMenuItems

                                            ] @ (user.IsInRole [ Role.Admin; Role.Trener ] =?
                                                    ([
                                                        hr [ _class "visible-xs submenu-divider" ]
                                                        ul [ _class "nav navbar-nav submenu visible-xs adminMenu" ]
                                                            coachMenuItems
                                                        ], [])))

                                            ) []
                            ))
                            loginPartial user
                            user |> Option.fold (fun _ user ->
                                                    ul [ _id "submenu-internal"; _class "nav navbar-nav submenu slide-down-child hidden-xs collapse" ]
                                                        (internalMenuItems @
                                                         [
                                                            user.IsInRole [ Role.Admin; Role.Trener ] =?
                                                                (li [] [ a [ _href "/admin" ] [ !!(Icons.coach ""); encodedText " Admin" ] ]
                                                                , emptyText)
                                                         ])

                                ) emptyText
                        ]
                    ]
                    div [ _class "mt-page-header" ] [
                        div [ _class "container" ] [
                            h1 [] [
                                encodedText o.Title
                            ]
                        ]
                    ]
                    div [ _id "main-container"; _class "container" ]
                        content

                    script [ _src "/compiled/lib/lib.bundle.js?v23" ] []
                    script [ _src "/compiled/app.js?v23" ] []
                    script [ _src "/compiled/client/main.js?v1" ] []
            ] @ o.Scripts)
        ]



