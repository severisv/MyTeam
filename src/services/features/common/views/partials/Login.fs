namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam
open Microsoft.AspNetCore.Http

[<AutoOpen>]
module Login =

    let loginPartial (ctx: HttpContext) =
        if ctx.User.Identity.IsAuthenticated then
            form [] []
            // <form asp-controller="Account" asp-action="LogOff" method="post" id="logoutForm" class="navbar-topRight--item navbar-right signout-form">
            //     <ul class="nav navbar-nav navbar-right">
            //         @*<li>
            //             <a asp-controller="Manage" asp-action="Index" title="@Res.User"><i class="fa fa-user"></i></a>
            //         </li>*@
            //         <li>
            //             <a href="javascript:document.getElementById('logoutForm').submit()" title="@Res.Logout"><i class="fa fa-sign-out"></i></a>
            //         </li>
            //     </ul>
            // </form>

        else
            ul [_id "login-wrapper"; _class "nav navbar-nav navbar-right navbar-topRight--item" ] [

                li [] [
                    a [_href <| sprintf "account/login?returnUrl=%s" (ctx.Request.Path + ctx.Request.QueryString) ] [
                        icon <| fa "sign-in"
                    ]
                ]
            ]




    let userPartial ctx notifications user   =
        let getImage = Images.getMember ctx
        let loginPartial = loginPartial ctx
                            
        div [_class "navbar-topRight"] [
            loginPartial
            user |> Option.fold (fun _ (user: Users.User) ->
                                            div [_class "login-image-wrapper"] [
                                                a [_title user.Name; _href <| "/spillere/vis/" + user.UrlName ] [
                                                    img [_src <| getImage user.Image user.FacebookId (fun o -> { o with Height = Some 40; Width = Some 40 }) ]
                                                ]
                                            ]
                            ) emptyText
            user |> Option.fold (fun _ (user: Users.User) ->
                                        notifications user                                                                        
                            ) emptyText
        ]                                                  





