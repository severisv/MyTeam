module MyTeam.Account.ResetPassword

open System.Web
open Giraffe.ViewEngine
open MyTeam
open Services.Utils
open Shared
open Shared.Validation
open MyTeam.Validation
open Shared.Components
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Domain
open Shared.Domain.Members
open Giraffe
open Fable.React.Props
open Microsoft.AspNetCore.Identity
open MyTeam.Models


[<CLIMutable>]
type ResetForm = { ``E-post``: string }


let view model (errors: ValidationError list) (club: Club) (user: User option) (ctx: HttpContext) =

    let validationMessage name =
        Forms.validationMessage (
            errors
            |> List.filter (fun ve -> ve.Name = name)
            |> List.collect (fun ve -> ve.Errors)
        )

    [ mtMain [ _class "mt-main--narrow" ] [
          block [] [
              form [ _method "post"
                     _class "form-horizontal"
                     _action "/konto/glemt-passord"
                     attr "" "novalidate" ] [
                  !!(Forms.formRow [ Forms.Horizontal 3 ] [] [ validationMessage "" ])
                  !!(Forms.formRow
                      [ Forms.Horizontal 3 ]
                      [ Fable.React.Helpers.str "E-post" ]
                      [ Forms.textInput [ Name "E-post"
                                          Value(
                                              (model
                                               |> Option.map (fun model -> model.``E-post``)
                                               |> Option.defaultValue "")
                                          ) ]
                        validationMessage "E-post" ])

                  antiforgeryToken ctx
                  !!(Forms.formRow
                      [ Forms.Horizontal 3 ]
                      []
                      [ btn [ Type "submit"; Primary ] [
                            Fable.React.Helpers.str "Nullstill passord"
                        ] ])
              ]
          ]
      ] ]
    |> layout club user (fun o -> { o with Title = "Glemt passord" }) ctx
    |> OkResult



let post (club: Club) (user: User option) form (ctx: HttpContext) =
    form
    |> function
        | Ok form ->
            combine [ <@ form.``E-post`` @>
                      >- [ isRequired; isValidEmail ] ]
            |> function
                | Ok _ ->

                    let userManager =
                        ctx.GetService<UserManager<ApplicationUser>>()


                    let sendMail = Email.send ctx.RequestServices

                    task {
                        let! account = userManager.FindByNameAsync(form.``E-post``)

                        if isNull account then
                            return
                                view
                                    (Some form)
                                    [ { Name = ""
                                        Errors = [ sprintf "Finner ikke ikke noen bruker for %s" form.``E-post`` ] } ]
                                    club
                                    user
                                    ctx
                        else
                            let! code = userManager.GeneratePasswordResetTokenAsync account

                            let callbackUrl =
                                sprintf
                                    "%s://%s/konto/nullstill-passord?code=%s&email=%s"
                                    ctx.Request.Scheme
                                    (ctx.Request.Host.ToString())
                                    (HttpUtility.UrlEncode(code))
                                    (HttpUtility.UrlEncode(account.Email))


                            do!
                                sendMail
                                    form.``E-post``
                                    "Nullstill passord"
                                    ("Du kan nullstille passordet ditt ved å trykke <a href=\""
                                     + callbackUrl
                                     + "\">her</a>")

                            return
                                [ mtMain [] [
                                      block [] [
                                          br []
                                          p [] [
                                              !!(Icons.check)
                                              encodedText
                                                  " En e-post med instruksjoner om nullstilling av passord er sent"
                                          ]
                                      ]
                                  ] ]
                                |> layout club user (fun o -> { o with Title = "Glemt passord" }) ctx
                                |> OkResult
                    }
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                | Error e -> view (Some form) e club user ctx

        | Error e -> failwith e



[<CLIMutable>]
type ConfirmResetForm =
    { ``E-post``: string
      Passord: string
      ``Bekreft passordet``: string
      Code: string }


let confirmView
    (model: ConfirmResetForm option)
    (errors: ValidationError list)
    (club: Club)
    (user: User option)
    (ctx: HttpContext)
    =

    let code, email =
        match model with
        | Some model -> Some model.Code, Some model.``E-post``
        | _ -> ctx.TryGetQueryStringValue "code", ctx.TryGetQueryStringValue "email"

    match (code, email) with
    | (Some code, Some email) ->
        let validationMessage name =
            Forms.validationMessage (
                errors
                |> List.filter (fun ve -> ve.Name = name)
                |> List.collect (fun ve -> ve.Errors)
            )

        [ mtMain [ _class "mt-main--narrow" ] [
              block [] [
                  h4 [] [ str "Nytt passord" ]
                  form [ _method "post"
                         _class "form-horizontal"
                         _action "/konto/nullstill-passord"
                         attr "" "novalidate" ] [

                      input [ _name "E-post"
                              _value email
                              _type "hidden" ]
                      input [ _name "Code"
                              _value code
                              _type "hidden" ]
                      !!(Forms.formRow [ Forms.Horizontal 3 ] [] [ validationMessage "" ])
                      !!(Forms.formRow
                          [ Forms.Horizontal 3 ]
                          [ Fable.React.Helpers.str "Passord" ]
                          [ Forms.textInput [ Type "password"
                                              Name "Passord"
                                              Value ""
                                              AutoComplete "off" ]
                            validationMessage "Passord" ]

                      )
                      !!(Forms.formRow
                          [ Forms.Horizontal 3 ]
                          [ Fable.React.Helpers.str "Bekreft passordet" ]
                          [ Forms.textInput [ Type "password"
                                              Name "Bekreft passordet"
                                              Value ""
                                              AutoComplete "off" ]
                            validationMessage "Bekreft passordet" ])

                      antiforgeryToken ctx
                      !!(Forms.formRow
                          [ Forms.Horizontal 3 ]
                          []
                          [ btn [ Type "submit"; Primary ] [
                                Fable.React.Helpers.str "Bekreft"
                            ] ])
                  ]
              ]
          ] ]
        |> layout club user (fun o -> { o with Title = "Lag nytt passord" }) ctx
        |> OkResult
    | _ -> failwithf "Ugyldig passord-nullstilling. Kode: %O  E-post: %O" code email



let confirmPost (club: Club) (user: User option) form (ctx: HttpContext) =

    form
    |> function
        | Ok form ->
            combine [ <@ form.``E-post`` @>
                      >- [ isRequired; isValidEmail ]
                      <@ form.Passord @> >- [ isRequired; minLength 6 ]
                      <@ form.``Bekreft passordet`` @> >- [ isRequired ]
                      <@ form @>
                      >- [ fun name value ->
                               if value.Passord = value.``Bekreft passordet`` then
                                   Ok()
                               else
                                   Error "Passordene stemmer ikke overrens" ] ]
            |> function
                | Ok _ ->

                    let userManager =
                        ctx.GetService<UserManager<ApplicationUser>>()

                    task {

                        let! account = userManager.FindByNameAsync(form.``E-post``)

                        if isNull account then
                            failwithf "Forsøkte å nullstille passord, men fant ikke brukeren: %O" form

                        let! result = userManager.ResetPasswordAsync(account, form.Code, form.Passord)

                        match result.Succeeded with
                        | true ->
                            return
                                [ mtMain [] [
                                      block [] [
                                          br []
                                          p [] [
                                              encodedText "Passordet ditt er nullstilt. "
                                              a [ _href "/konto/innlogging" ] [
                                                  encodedText "Logg inn"
                                              ]
                                          ]
                                      ]
                                  ] ]
                                |> layout club user (fun o -> { o with Title = "Passord nullstilt" }) ctx
                                |> OkResult
                        | false ->
                            return
                                confirmView
                                    (Some form)
                                    (result.Errors
                                     |> Seq.map
                                         (fun e ->
                                             { Name = ""
                                               Errors = [ e.Description ] })
                                     |> Seq.toList)
                                    club
                                    user
                                    ctx
                    }
                    |> Async.AwaitTask
                    |> Async.RunSynchronously
                | Error e -> confirmView (Some form) e club user ctx

        | Error e -> failwith e
