module MyTeam.Account.Login

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Components
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Domain
open Shared.Domain.Members
open Server
open Giraffe
open System.Linq
open System


let view (club: Club) (user: User option) (ctx: HttpContext) =

    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"

    match (ctx.User.Identity.IsAuthenticated, returnUrl) with
    | (true, Some returnUrl) when returnUrl
                                  |> Strings.equalsIgnoreCase (string ctx.Request.Path)
                                  |> not -> Redirect returnUrl
    | (true, _) -> Redirect "/"
    | (_, _) ->
        let returnUrl =
            (returnUrl
             |> Option.map (sprintf "?returnurl=%s")
             |> Option.defaultValue "")

        [ mtMain [ _class "mt-main--narrow"; _id "mt-login" ] [
            block [] [
                form [ _method "post"
                       _class "form-horizontal"
                       _action
                       <| sprintf "/konto/innlogging/ekstern%s" returnUrl ] [
                    div [] [
                        div [ _class "login--providerLogin text-center" ] [
                            br []
                            br []
                            !!(btn [ 
                                     Fable.React.Props.Name "provider"
                                     Fable.React.Props.Value "Facebook"
                                     Fable.React.Props.Type "submit"
                                     Primary
                                     Lg ] [
                                 Icons.facebook ""
                                 Fable.React.Helpers.str " Logg inn med Facebook"
                            ])
                          
                        ]
                    ]
                    antiforgeryToken ctx
                ]
                br []
                br []
                hr []
                br []
                br []
                h4 [] [str "Logg inn med lokal brukerkonto"]
                form [ _method "post"
                       _class "form-horizontal"
                       _action
                       <| sprintf
                           "/konto/innlogging%s"
                              returnUrl
                       attr "" "novalidate" ] [
                    div [ _class "col-md-offset-2 col-md-10" ] [
                        div [ _class "text-danger validation-summary-valid"
                              attr "data-valmsg-summary" "true" ] [
                            ul [] [ li [] [] ]
                        ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "col-md-2 control-label" ] [
                            str "E-post"
                        ]
                        div [ _class "col-md-10" ] [
                            input [ _class "form-control"
                                    _type "email"                                   
                                    _id "Email"
                                    _name "Email"
                                    _value ""
                                    attr "autocomplete" "off" ]
                            span [ _class "text-danger field-validation-valid"
                                   attr "valmsg-for" "Email"
                                   attr "valmsg-replace" "true" ] []
                        ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "col-md-2 control-label" ] [
                            str "Passord"
                        ]
                        div [ _class "col-md-10" ] [
                            input [ _class "form-control"
                                    _type "password"                                
                                    _id "Password"
                                    _name "Password"
                                    attr "autocomplete" "off" ]
                            span [ _class "text-danger field-validation-valid"
                                   attr "valmsg-for" "Password"
                                   attr "valmsg-replace" "true" ] []
                        ]
                    ]
                    div [ _class "form-group" ] [
                        div [ _class "col-md-offset-2 col-xs-offset-0 col-md-10" ] [
                            div [ _class "checkbox" ] [
                                input [ _type "checkbox"                                     
                                        _id "RememberMe"
                                        _name "RememberMe"
                                        _value "true" ]
                                label [attr "for" "RememberMe"] [ str "Husk meg" ]
                            ]
                        ]
                    ]
                    antiforgeryToken ctx

                    div [ _class "form-group" ] [
                        div [ _class "col-md-offset-2 col-md-10" ] [
                            !!(btn [ Fable.React.Props.Type "submit"
                                     Primary
                                     Lg ] [
                                 Fable.React.Helpers.str "Logg in"
                            ])
                        ]
                    ]
                       ]
                br []
                p [ _class "col-md-offset-2 col-sm-offset-0" ] [
                    a [ _href <| sprintf "/konto/ny%s" returnUrl ] [
                        str "Registrer ny bruker"
                    ]
                ]
                p [ _class "col-md-offset-2 col-sm-offset-0" ] [
                    a [ _href <| sprintf "/konto/passord/glemt%s" returnUrl ] [
                        str "Glemt passordet?"
                    ]
                ]
                    
                
            ]
          ] ]
        |> layout club user (fun o ->
               { o with
                     Title = "Logg inn"
                     Scripts = [] }) ctx
        |> OkResult
