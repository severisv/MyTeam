module MyTeam.Account.Signup

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
open Giraffe
open Fable.React.Props
open Microsoft.AspNetCore.Identity
open MyTeam.Models
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive


[<CLIMutable>]
type SignupForm =
    { ``E-post``: string
      Passord: string
      ``Bekreft passordet``: string }


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

        [ mtMain [ _class "mt-main--narrow" ] [
            block [] [
                h4 [] [ str "Opprett en brukerkonto" ]
                form [ _method "post"
                       _class "form-horizontal"
                       _action
                       <| sprintf "/kontoz/ny%s" returnUrl
                       attr "" "novalidate" ] [
                    !!(Forms.formRow [ Forms.Horizontal 3 ] [] [ validationMessage "" ])
                    !!(Forms.formRow
                        [ Forms.Horizontal 3 ]
                           [ Fable.React.Helpers.str "E-post" ]
                           [ Forms.textInput [ Name "E-post"
                                               Value
                                                   ((model
                                                     |> Option.map (fun model -> model.``E-post``)
                                                     |> Option.defaultValue ""))
                                               AutoComplete "off" ]
                             validationMessage "E-post" ])
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
                               Fable.React.Helpers.str "Opprett konto"
                             ] ])
                ]
            ]
          ] ]
        |> layout club user (fun o -> { o with Title = "Registrering" }) ctx
        |> OkResult



let post (club: Club) (user: User option) form (ctx: HttpContext) =
    let returnUrl = ctx.TryGetQueryStringValue "returnUrl"
    let logger = Logger.get ctx.RequestServices


    form
    |> function
    | Ok form ->
        combine [ <@ form.``E-post`` @> >- [ isRequired; isValidEmail ]
                  <@ form.Passord @> >- [ isRequired; minLength 6 ]
                  <@ form.``Bekreft passordet`` @> >- [ isRequired ]
                  <@ form @>
                  >- [ fun name value ->
                           if value.Passord = value.``Bekreft passordet``
                           then Ok()
                           else Error "Passordene stemmer ikke overrens" ] ]
        |> function
        | Ok _ ->


            let signInManager =
                ctx.GetService<SignInManager<ApplicationUser>>()

            let userManager =
                ctx.GetService<UserManager<ApplicationUser>>()

            task {
                let au =
                    ApplicationUser(UserName = form.``E-post``, Email = form.``E-post``)

                let! result = userManager.CreateAsync(au, form.Passord)

                match result.Succeeded with
                | true ->
                    do! signInManager.SignInAsync(au, true)
                    logger.LogInformation
                        (EventId(3), (sprintf "User %s created a new account with password." form.``E-post``))
                    return Redirect(returnUrl |> Option.defaultValue "/")
                | false ->
                    return view
                               (Some form)
                               (result.Errors
                                |> Seq.map (fun e ->
                                    { Name = ""
                                      Errors = [ e.Description ] })
                                |> Seq.toList)
                               club
                               user
                               ctx
            }
            |> Async.AwaitTask
            |> Async.RunSynchronously
        | Error e -> view (Some form) e club user ctx

    | Error e -> failwith e
