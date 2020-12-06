module MyTeam.Account.Login

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Validation
open MyTeam.Validation
open Shared.Components
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Domain
open Shared.Domain.Members
open Server
open Giraffe
open Fable.React.Props
open Microsoft.AspNetCore.Identity
open MyTeam.Models
open Server.Common
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Security.Claims
open Microsoft.AspNetCore.Authentication



[<CLIMutable>]
type LoginForm =
    { Epost: string
      Passord: string
      HuskMeg: string option }


let view model (errors: ValidationError list) (club: Club) (user: User option) (ctx: HttpContext) =

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


        let validationMessage name =
            Forms.validationMessage
                (errors
                 |> List.filter (fun ve -> ve.Name = name)
                 |> List.collect (fun ve -> ve.Errors))

        [ mtMain [ _class "mt-main--narrow"
                   _id "mt-login" ] [
            block [] [
                form [ _method "post"
                       _class "form-horizontal"
                       _action
                       <| sprintf "/kontoz/innlogging/ekstern%s" returnUrl ] [
                    div [] [
                        div [ _class "login--providerLogin text-center" ] [
                            br []
                            br []
                            !!(btn [ Name "provider"
                                     Value "Facebook"
                                     Type "submit"
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
                h4 [] [
                    str "Logg inn med lokal brukerkonto"
                ]
                form [ _method "post"
                       _class "form-horizontal"
                       _action
                       <| sprintf "/kontoz/innlogging%s" returnUrl
                       attr "" "novalidate" ] [
                    !!(Forms.formRow [ Forms.Horizontal 2 ] [] [ validationMessage "" ])
                    !!(Forms.formRow
                        [ Forms.Horizontal 2 ]
                           [ Fable.React.Helpers.str "E-post" ]
                           [ Forms.textInput [ Name "Epost"
                                               Value
                                                   ((model
                                                     |> Option.map (fun model -> model.Epost)
                                                     |> Option.defaultValue ""))
                                               AutoComplete "off" ]
                             validationMessage "Epost" ])
                    !!(Forms.formRow
                        [ Forms.Horizontal 2 ]
                           [ Fable.React.Helpers.str "Passord" ]
                           [ Forms.textInput [ Type "password"
                                               Name "Passord"
                                               Value ""
                                               AutoComplete "off" ]
                             validationMessage "Passord" ]

                    )
                    !!(Forms.formRow
                        [ Forms.Horizontal 2 ]
                           []
                           [ Forms.checkboxInput
                               [ Name "HuskMeg" ]
                                 [ Fable.React.Helpers.str "Husk meg" ]
                                 (model
                                  |> Option.map (fun model -> model.HuskMeg.IsSome)
                                  |> Option.defaultValue true)
                                 ignore ])

                    antiforgeryToken ctx
                    !!(Forms.formRow
                        [ Forms.Horizontal 2 ]
                           []
                           [ btn [ Type "submit"; Primary ] [
                               Fable.React.Helpers.str "Logg in"
                             ] ])
                ]
                br []
                p [ _class "col-md-offset-2 col-sm-offset-0" ] [
                    a [ _href <| sprintf "/kontoz/ny%s" returnUrl ] [
                        str "Registrer ny bruker"
                    ]
                ]
                p [ _class "col-md-offset-2 col-sm-offset-0" ] [
                    a [ _href
                        <| sprintf "/konto/passord/glemt%s" returnUrl ] [
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



let post (club: Club) (user: User option) form (ctx: HttpContext) =
    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"
    let logger = Logger.get ctx.RequestServices


    form
    |> function
    | Ok form ->
        combine [ <@ form.Epost @> >- [ isRequired; isValidEmail ]
                  <@ form.Passord @> >- [ isRequired ] ]
        |> function
        | Ok _ ->


            let signInManager =
                ctx.GetService<SignInManager<ApplicationUser>>()


            let result =
                async {
                    return! (signInManager.PasswordSignInAsync(form.Epost, form.Passord, form.HuskMeg.IsSome, false))
                            |> Async.AwaitTask
                }
                |> Async.RunSynchronously


            match (result.Succeeded, result.IsLockedOut) with
            | (true, _) ->
                Tenant.clearUserCache ctx club.Id (UserId form.Epost)
                logger.LogDebug(EventId(), "User logged in.")

                Redirect(returnUrl |> Option.defaultValue "/")

            | (_, true) ->
                logger.LogWarning(EventId(), "User account locked out.")

                [ mtMain [] [
                    block [] [
                        h2 [] [
                            encodedText "Denne kontoen har blitt låst"
                        ]
                    ]
                  ] ]
                |> layout club user (fun o ->
                       { o with
                             Title = "Logg inn"
                             Scripts = [] }) ctx
                |> OkResult

            | (_, _) ->
                view
                    (Some form)
                    [ { Name = ""
                        Errors = [ "Brukernavn og passord stemmer ikke overens" ] } ]
                    club
                    user
                    ctx



        | Error e -> view (Some form) e club user ctx
    | Error e -> failwith e



let external : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let provider = "Facebook"

            let returnUrl =
                (ctx.TryGetQueryStringValue "returnUrl"
                 |> Option.map (sprintf "?returnurl=%s")
                 |> Option.defaultValue "")

            let items =
                System.Collections.Generic.Dictionary<string, string>()
            
            items.Add("LoginProvider", provider)
                
            let props = AuthenticationProperties(items, RedirectUri = (sprintf "/kontoz/innlogging/ekstern%s" returnUrl))
            
            do! ctx.ChallengeAsync(provider, props)
            return! next ctx
        }




let externalCallback club user (ctx: HttpContext) =
    task {
        let log = Logger.get ctx.RequestServices

        let returnUrl =
            ctx.TryGetQueryStringValue "returnUrl"
            |> Option.defaultValue "/"

        let signInManager =
            ctx.GetService<SignInManager<ApplicationUser>>()

        let userManager =
            ctx.GetService<UserManager<ApplicationUser>>()


        let! info = signInManager.GetExternalLoginInfoAsync()


        if isNull info then       
            return Redirect "/kontoz/innlogging"
        else

            // Sign in the user with this external login provider if the user already has a login.
            let! result = signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true)

            if result.Succeeded then

                log.LogDebug(EventId(), "User logged in with {Name} provider.", info.LoginProvider)


                let! user = userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)

                do! signInManager.SignInAsync(user, true)

                let addClaim name claimType =
                    match (info.Principal.Claims
                           |> Seq.toList
                           |> List.tryFind (fun c -> c.Type = name),
                           info.Principal.Claims
                           |> Seq.toList
                           |> List.tryFind (fun c -> c.Type = claimType)) with
                    | (None, Some claim) ->
                        task {
                            let! _ = userManager.AddClaimAsync(user, Claim(name, claim.Value))
                            ()
                        }
                    | (_, _) -> task { () }

                do! addClaim "facebookFirstName" ClaimTypes.GivenName

                do! addClaim "facebookLastName" ClaimTypes.Surname
                do! addClaim "facebookId" ClaimTypes.NameIdentifier
                
                return Redirect returnUrl

            else if result.IsLockedOut then
                return [ encodedText "Lockout" ]
                       |> layout club user (fun o -> { o with Title = "Lockout" }) ctx
                       |> OkResult

            else
                return Redirect "/innlogging/ekstern/bekreftelse"



    }
    |> Async.AwaitTask
    |> Async.RunSynchronously



let logOut (club: Club) user (ctx: HttpContext ) =
        task {
            
            let log = Logger.get ctx.RequestServices

            let returnUrl =
                ctx.TryGetQueryStringValue "returnUrl"
                |> Option.defaultValue "/"

            let signInManager = ctx.GetService<SignInManager<ApplicationUser>>()
            do! signInManager.SignOutAsync()
            log.LogDebug(EventId(4), "User logged out.");
            
            Tenant.clearUserCache ctx club.Id (UserId ctx.User.Identity.Name)

            
            return Redirect returnUrl
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously