namespace MyTeam

open Giraffe
open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Options
open MyTeam.Views
open MyTeam.Views.BaseComponents


[<AutoOpen>]
module Pages =

    let internal coachMenuItems =
        [ ("/treninger/ny", Icons.training "", "Ny trening")
          ("/kamper/ny", Icons.game "", "Ny kamp")
          ("/admin", Icons.user "", "Administrer spillere")
          ("/admin/spillerinvitasjon", Icons.add "", "Legg til spiller")
          ("/nyheter/ny", Icons.news "", "Skriv artikkel") ]
        |> List.map (fun (href, icon, text) ->
            li [] [
                a [ _href href ] [
                    !!icon
                    encodedText text
                ]
            ])

    let internal internalMenuItems =
        [ ("/intern", Icons.signup "", "Påmelding")
          ("/intern/oppmote", Icons.attendance "", "Oppmøte")
          ("/intern/boter/oversikt", Icons.fine "", "Bøter")
          ("/intern/lagliste", Icons.squadList "", "Lagliste") ]
        |> List.map (fun (href, icon, text) ->
            li [] [
                a [ _href href ] [
                    !!icon
                    encodedText <| sprintf " %s" text
                ]
            ])


    type LayoutModel =
        { Title: string
          MetaTitle: string
          MetaDescription: string
          MetaImage: string
          Scripts: XmlNode list }

    let layout (club: Club) (user: Option<User>) getOptions (ctx: HttpContext) content =

        let isProduction = ctx.GetService<IHostEnvironment>().IsProduction()

        let assetHashes = ctx.GetService<IOptions<AssetHashes>>().Value

        let o =
            getOptions (
                { Title = ""
                  MetaTitle = ""
                  MetaDescription = ""
                  MetaImage = ""
                  Scripts = [] }
            )

        let getImage = Images.get ctx

        html [ _lang "nb-no" ] [
            head [] [
                meta [ _charset "utf-8" ]
                meta [
                    _name "viewport"
                    _content "width=device-width, initial-scale=1.0"
                ]
                meta [
                    _title
                    <| club.ShortName
                       + (Strings.hasValue o.Title =? (" - " + o.Title, ""))
                ]
                Strings.hasValue o.MetaDescription
                =? (meta [
                        _name "description"
                        _content o.MetaDescription
                    ],
                    emptyText)
                Strings.hasValue o.MetaImage
                =? (meta [
                        _property "og:image"
                        _content o.MetaImage
                    ],
                    emptyText)

                title [] [
                    encodedText (
                        o.MetaTitle
                        =?? (sprintf "%s - %s" club.Name o.Title)
                    )
                ]
                link [
                    _rel "stylesheet"
                    _href
                    <| sprintf "/compiled/site.bundle.css?v%s" assetHashes.MainCss
                ]
                link [
                    _rel "icon"
                    _type "image/png"
                    _href <| getImage id (club.Favicon =?? club.Logo)
                ]
                link [
                    _rel "apple-touch-icon"
                    _type "image/png"
                    _href <| getImage id (club.Favicon =?? club.Logo)
                ]
                isProduction =? (Analytics.script, empty)
            ]
            body
                []
                ([ div [ _class "navbar navbar-inverse navbar-fixed-top" ] [
                       div [ _class "container" ] [
                           div [ _class "navbar-header" ] [
                               button [ _type "button"
                                        _class "navbar-toggle"
                                        attr "data-toggle" "collapse"
                                        attr "data-target" ".navbar-collapse" ] [
                                   span [ _class "icon-bar" ] []
                                   span [ _class "icon-bar" ] []
                                   span [ _class "icon-bar" ] []
                               ]
                               a [ _href "/"; _class "navbar-brand" ] [
                                   img [
                                       _src
                                       <| getImage (fun o -> { o with Width = Some 100 }) club.Logo
                                       _alt club.ShortName
                                   ]
                               ]
                           ]
                           div
                               [ _class "navbar-collapse collapse" ]
                               ([ ul [ _class "nav navbar-nav" ] [
                                      li [] [
                                          a [ _href "/spillere" ] [
                                              encodedText "Spillere"
                                          ]
                                      ]
                                      li [] [
                                          a [ _href "/kamper" ] [
                                              encodedText "Kamper"
                                          ]
                                      ]
                                      li [] [
                                          a [ _href "/tabell" ] [
                                              encodedText "Tabell"
                                          ]
                                      ]
                                      li [] [
                                          a [ _href "/statistikk" ] [
                                              encodedText "Statistikk"
                                          ]
                                      ]
                                      li [] [
                                          a [ _href "/om" ] [
                                              encodedText "Om"
                                              span [ _class "hidden-sm" ] [
                                                  encodedText " klubben"
                                              ]
                                          ]
                                      ]
                                      user
                                      |> Option.fold
                                          (fun _ _ ->
                                              li [] [
                                                  a [ _class "slide-down-parent hidden-xs"
                                                      attr "data-submenu" "#submenu-internal"
                                                      _href "javascript:void(0)" ] [
                                                      encodedText "Intern "
                                                      icon <| fa "angle-down" <| ""
                                                  ]
                                              ]

                                              )
                                          (emptyText)
                                  ] ]
                                @ (user
                                   |> Option.fold
                                       (fun _ user ->
                                           ([ hr [
                                                  _class "visible-xs submenu-divider"
                                              ]
                                              ul [ _class "nav navbar-nav submenu visible-xs" ] internalMenuItems ]
                                            @ (user.IsInRole [
                                                Role.Admin
                                                Role.Trener
                                               ]
                                               =? ([ hr [
                                                         _class "visible-xs submenu-divider"
                                                     ]
                                                     ((ul [ _class "nav navbar-nav submenu visible-xs adminMenu" ] coachMenuItems)) ],
                                                   [])))

                                           )
                                       []))
                           userPartial club user ctx
                           user
                           |> Option.fold
                               (fun _ user ->
                                   ul
                                       [ _id "submenu-internal"
                                         _class "nav navbar-nav submenu slide-down-child hidden-xs collapse" ]
                                       (internalMenuItems
                                        @ [ user.IsInRole [
                                                Role.Admin
                                                Role.Trener
                                            ]
                                            =? (li [] [
                                                    a [ _href "/admin" ] [
                                                        !!(Icons.coach "")
                                                        encodedText " Admin"
                                                    ]
                                                ],
                                                emptyText) ]))
                               emptyText
                       ]
                   ]
                   div [ _class "mt-page-header" ] [
                       div [ _class "container" ] [
                           h1 [] [ encodedText o.Title ]
                       ]
                   ]
                   div
                       [ _id "main-container"
                         _class "container" ]
                       content

                   footer [ _style "height:0;position: relative;" ] [
                       a [ _href "/personvern"
                           _style "position: absolute;bottom: 0;color: transparent !important;font-size: 5px;"
                           _tabindex "-1" ] [
                           str "Privacy"
                       ]
                   ]

                   script [ _src
                            <| sprintf "/compiled/lib/lib.bundle.js?v%s" assetHashes.LibJs ] []
                   script [ _src
                            <| sprintf "/compiled/app.js?v%s" assetHashes.MainJs ] []
                   script [ _src
                            <| sprintf "/compiled/client/main.js?v%s" assetHashes.FableJs ] [] ]
                 @ o.Scripts)
        ]
