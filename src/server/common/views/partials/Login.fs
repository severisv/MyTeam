namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain.Members
open Microsoft.AspNetCore.Http
open MyTeam.Views.BaseComponents

[<AutoOpen>]
module Login =

    let loginPartial (ctx: HttpContext) =
        if ctx.User.Identity.IsAuthenticated then
            form [_action <| sprintf "/konto/utlogging?returnUrl=%s" (ctx.Request.Path + ctx.Request.QueryString);_method "POST";_id "logoutForm";_class "navbar-topRight--item navbar-right signout-form"] [
                antiforgeryToken ctx
                ul [_class "nav navbar-nav navbar-right"] [
                    li [] [
                        a [_href "javascript:document.getElementById('logoutForm').submit()";_title "Logg ut"] [
                            icon <| fa "sign-out" <| "Logg ut"
                    ]
                    ]
                ]
            ]
        else
            ul [_id "login-wrapper"; _class "nav navbar-nav navbar-right navbar-topRight--item" ] [
                li [] [
                    a [_href <| sprintf "/konto/innlogging?returnUrl=%s" (ctx.Request.Path + ctx.Request.QueryString) ] [
                        icon <| fa "sign-in" <| "Logg inn"
                    ]
                ]
            ]

    let userPartial ctx notifications user   =
        let getImage = Images.getMember ctx
        let loginPartial = loginPartial ctx
                            
        div [_class "navbar-topRight"] [
            loginPartial
            user |> Option.fold (fun _ (user: User) ->
                                            div [_class "login-image-wrapper"] [
                                                a [_title user.Name; _href <| "/spillere/vis/" + user.UrlName ] [
                                                    img [_src <| getImage (fun o -> { o with Height = Some 40; Width = Some 40 }) user.Image user.FacebookId  ]
                                                ]
                                            ]
                            ) emptyText
            user |> Option.fold (fun _ (user: User) ->
                                        notifications user                                                                        
                            ) emptyText
        ]                                                  





