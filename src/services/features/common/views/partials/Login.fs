namespace MyTeam.Views

open Giraffe.GiraffeViewEngine
open MyTeam

[<AutoOpen>]
module Login =

    let loginPartial ctx notifications user   =
        let getImage = Images.getMember ctx
        user |> Option.fold (fun _ (user: Users.User) ->
                                        span [] [
                                            div [_class "login-image-wrapper"] [
                                                a [_title user.Name; _href <| "/spillere/vis/" + user.UrlName ] [
                                                    img [_src <| getImage user.Image user.FacebookId (fun o -> { o with Height = Some 40; Width = Some 40 }) ]
                                                ]
                                            ]
                                            notifications user
                                        ]                                      
                            ) emptyText