module MyTeam.Account.Login

open Giraffe.ViewEngine
open Microsoft.AspNetCore.Identity
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
open MyTeam.Models
open Server.Common
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks
open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open System.Linq



[<CLIMutable>]
type LoginForm =
    { Epost: string
      Passord: string
      HuskMeg: string option }


let view model (errors: ValidationError list) (club: Club) (user: User option) (ctx: HttpContext) =

    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"

    match (ctx.User.Identity.IsAuthenticated, returnUrl) with
    | (true, Some returnUrl) when
        returnUrl
        |> Strings.equalsIgnoreCase (string ctx.Request.Path)
        |> not
        ->
        Redirect returnUrl
    | (true, _) -> Redirect "/"
    | (_, _) ->
        let returnUrl =
            (returnUrl
             |> Option.map (sprintf "?returnurl=%s")
             |> Option.defaultValue "")


        let validationMessage name =
            Forms.validationMessage (
                errors
                |> List.filter (fun ve -> ve.Name = name)
                |> List.collect (fun ve -> ve.Errors)
            )

        [ mtMain [ _class "mt-main--narrow"
                   _id "mt-login" ] [
              block [] [
                  form [ _method "post"
                         _class "form-horizontal"
                         _action
                         <| sprintf "/konto/innlogging/ekstern%s" returnUrl ] [
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
                         _action <| sprintf "/konto/innlogging%s" returnUrl
                         attr "" "novalidate" ] [
                      !!(Forms.formRow [ Forms.Horizontal 2 ] [] [ validationMessage "" ])
                      !!(Forms.formRow
                          [ Forms.Horizontal 2 ]
                          [ Fable.React.Helpers.str "E-post" ]
                          [ Forms.textInput [ Name "Epost"
                                              Value(
                                                  (model
                                                   |> Option.map (fun model -> model.Epost)
                                                   |> Option.defaultValue "")
                                              )
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
                      a [ _href <| sprintf "/konto/ny%s" returnUrl ] [
                          str "Registrer ny bruker"
                      ]
                  ]
                  p [ _class "col-md-offset-2 col-sm-offset-0" ] [
                      a [ _href
                          <| sprintf "/konto/glemt-passord%s" returnUrl ] [
                          str "Glemt passordet?"
                      ]
                  ]
              ]
          ] ]
        |> layout
            club
            user
            (fun o ->
                { o with
                      Title = "Logg inn"
                      Scripts = [] })
            ctx
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
                            return!
                                (signInManager.PasswordSignInAsync(form.Epost, form.Passord, form.HuskMeg.IsSome, false))
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
                        |> layout
                            club
                            user
                            (fun o ->
                                { o with
                                      Title = "Logg inn"
                                      Scripts = [] })
                            ctx
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
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let provider = "Facebook"

            let returnUrl =
                (ctx.TryGetQueryStringValue "returnUrl"
                 |> Option.map (sprintf "?returnurl=%s")
                 |> Option.defaultValue "")

            let items =
                System.Collections.Generic.Dictionary<string, string>()

            items.Add("LoginProvider", provider)

            let props =
                AuthenticationProperties(items, RedirectUri = (sprintf "/konto/innlogging/ekstern%s" returnUrl))

            do! ctx.ChallengeAsync(provider, props)
            return! next ctx
        }




[<CLIMutable>]
type SignupExternalForm = { ``E-post``: string }


let signupExternalView model (errors: ValidationError list) (club: Club) (user: User option) (ctx: HttpContext) =
    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"

    match (ctx.User.Identity.IsAuthenticated, returnUrl) with
    | (true, Some returnUrl) when
        returnUrl
        |> Strings.equalsIgnoreCase (string ctx.Request.Path)
        |> not
        ->
        Redirect returnUrl
    | (true, _) -> Redirect "/"
    | (_, _) ->
        let returnUrl =
            (returnUrl
             |> Option.map (sprintf "?returnurl=%s")
             |> Option.defaultValue "")


        let validationMessage name =
            Forms.validationMessage (
                errors
                |> List.filter (fun ve -> ve.Name = name)
                |> List.collect (fun ve -> ve.Errors)
            )

        [ mtMain [ _class "mt-main--narrow" ] [
              block [] [
                  h4 [] [ str "Opprett en brukerkonto" ]
                  form [ _method "post"
                         _class "form-horizontal"
                         _action
                         <| sprintf "/konto/innlogging/ekstern/ny%s" returnUrl
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
                                              )
                                              AutoComplete "off" ]
                            validationMessage "E-post" ])
                      antiforgeryToken ctx
                      !!(Forms.formRow
                          [ Forms.Horizontal 3 ]
                          []
                          [ btn [ Type "submit"; Primary ] [
                                Fable.React.Helpers.str "Opprett konto"
                            ] ])
                  ]
              ]
          ] ]
        |> layout club user (fun o -> { o with Title = "Registrering" }) ctx
        |> OkResult



let internal addClaim
    (info: ExternalLoginInfo)
    (userManager: UserManager<ApplicationUser>)
    (user: ApplicationUser)
    name
    claimType
    =
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


let signupExternal (club: Club) (user: User option) form (ctx: HttpContext) =
    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"
    let logger = Logger.get ctx.RequestServices


    form
    |> function
        | Ok form ->
            combine [ <@ form.``E-post`` @>
                      >- [ isRequired; isValidEmail ] ]
            |> function
                | Ok _ ->


                    let signInManager =
                        ctx.GetService<SignInManager<ApplicationUser>>()

                    let userManager =
                        ctx.GetService<UserManager<ApplicationUser>>()

                    task {


                        let! info = signInManager.GetExternalLoginInfoAsync()

                        if isNull info then
                            return Redirect "/konto/innlogging"
                        else

                            let au =
                                ApplicationUser(UserName = form.``E-post``, Email = form.``E-post``)

                            let! result = userManager.CreateAsync(au)

                            match result.Succeeded with
                            | true ->
                                let! result = userManager.AddLoginAsync(au, info)

                                logger.LogInformation(
                                    EventId(3),
                                    (sprintf "User %s created a new account with Facebook." form.``E-post``)
                                )

                                let facebookId =
                                    match info.Principal.Claims
                                          |> Seq.tryFind (fun c -> c.Type = ClaimTypes.NameIdentifier) with
                                    | Some facebookId -> facebookId.Value
                                    | None ->
                                        failwithf
                                            "Could't find facebookId when creating account from external. %O"
                                            form.``E-post``

                                let db = ctx.Database

                                let player =
                                    db.Members.FirstOrDefault(fun p -> p.FacebookId = facebookId)

                                if not <| isNull player then
                                    player.UserName <- form.``E-post``
                                    db.SaveChanges() |> ignore


                                if result.Succeeded then
                                    let addClaim = addClaim info userManager au
                                    do! addClaim "facebookFirstName" ClaimTypes.GivenName
                                    do! addClaim "facebookLastName" ClaimTypes.Surname
                                    do! addClaim "facebookId" ClaimTypes.NameIdentifier

                                    do! signInManager.SignInAsync(au, true)

                                    return Redirect(returnUrl |> Option.defaultValue "/")
                                else
                                    return Redirect(returnUrl |> Option.defaultValue "/")
                            | false ->
                                return
                                    signupExternalView
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
                | Error e -> signupExternalView (Some form) e club user ctx
        | Error e -> failwith e


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
            return Redirect "/konto/innlogging"
        else

            let! au = userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)

            if not <| isNull au then

                log.LogDebug(EventId(), "User logged in with {Name} provider.", info.LoginProvider)

                do! signInManager.SignInAsync(au, true)

                let addClaim = addClaim info userManager au
                do! addClaim "facebookFirstName" ClaimTypes.GivenName
                do! addClaim "facebookLastName" ClaimTypes.Surname
                do! addClaim "facebookId" ClaimTypes.NameIdentifier

                return Redirect returnUrl
            else
                return signupExternalView None [] club user ctx



    }
    |> Async.AwaitTask
    |> Async.RunSynchronously



let logOut (club: Club) user (ctx: HttpContext) =
    task {

        let log = Logger.get ctx.RequestServices

        let returnUrl =
            ctx.TryGetQueryStringValue "returnUrl"
            |> Option.defaultValue "/"

        let signInManager =
            ctx.GetService<SignInManager<ApplicationUser>>()

        do! signInManager.SignOutAsync()
        log.LogDebug(EventId(4), "User logged out.")

        Tenant.clearUserCache ctx club.Id (UserId ctx.User.Identity.Name)


        return Redirect returnUrl
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
