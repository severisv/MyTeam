module MyTeam.Members.Pages.RequestAccess

open Fable.Helpers.React.Props
open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Components.Buttons
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Members
open MyTeam.Shared.Components
open MyTeam.Shared.Components.Forms
open MyTeam.Validation
open MyTeam.Views

[<CLIMutable>]
type RequestAccessForm =
    { Fornavn : string
      FacebookId : string
      Mellomnavn : string
      Etternavn : string }

let internal inputRow name value (validationErrors : ValidationError list) =
    formRow [ Horizontal 2 ] [ Fable.Helpers.React.str name ] 
        [ textInput [ Name name
                      Value value ]
          validationMessage (validationErrors
                             |> List.filter (fun ve -> ve.Name = name)
                             |> List.collect (fun ve -> ve.Errors)) ]

let internal view model validationErrors (club : Club) (user : Users.User option) 
    (ctx : HttpContext) =
    let db = ctx.Database
    user
    |> function 
    | Some _ -> Redirect "/intern"
    | None -> 
        [ mtMain [] 
              [ block [] 
                    (match (ctx.User.Identity.IsAuthenticated, model) with
                     | (false, _) -> 
                         [ h4 [] [ encodedText "Velkommen!" ]
                           div [ _class "text-center" ] [ p [] 
                                                              [ encodedText 
                                                                <| sprintf 
                                                                       "Her kan du be om spillertilgang til %s." 
                                                                       club.ShortName ]
                                                          p [] [ encodedText "Først må du "
                                                                 
                                                                 a 
                                                                     [ _href 
                                                                           "konto/innlogging?returnUrl=/blimed" ] 
                                                                     [ encodedText "logge inn" ]
                                                                 encodedText "." ] ] ]
                     | (true, Some model) -> 
                         [ p [ _class "text-center" ] 
                               [ encodedText 
                                     "Registrer navnet ditt her, så får du en tilbakemelding så fort forespørselen er godkjent av noen i støtteapparatet." ]
                           br []
                           
                           !!(form [ Horizontal 2
                                     Action "/blimed"
                                     Method "POST" ] 
                                  [ Fable.Helpers.React.input [ Name "FacebookId"
                                                                Type "hidden"
                                                                Value model.FacebookId ]
                                    inputRow "Fornavn" model.Fornavn validationErrors
                                    inputRow "Mellomnavn" model.Mellomnavn validationErrors
                                    inputRow "Etternavn" model.Etternavn validationErrors
                                    
                                    formRow [ Horizontal 2 ] [] 
                                        [ btn [ Primary ] [ Fable.Helpers.React.str "Send" ] ] ]) ]
                     | (true, None) -> 
                         [ h4 [ _class "text-left" ] [ encodedText "Forespørselen din er mottatt" ]
                           
                           p [] 
                               [ encodedText 
                                     "Du får en tilbakemelding så fort forespørselen er godkjent av noen i støtteapparatet." ] ]) ] ]
        |> layout club user (fun o -> { o with Title = "Be om tilgang" }) ctx
        |> OkResult

let get (club : Club) user (ctx : HttpContext) =
    let (ClubId clubId) = club.Id
    ctx.Database.MemberRequests
    |> Seq.tryFind (fun mr -> mr.ClubId = clubId && mr.Email = ctx.User.Identity.Name)
    |> function 
    | Some _ -> view None [] club user ctx
    | None -> 
        let model =
            { Fornavn = ctx.User.GetClaim "facebookFirstName" |> Option.defaultValue ""
              Mellomnavn = ""
              Etternavn = ctx.User.GetClaim "facebookLastName" |> Option.defaultValue ""
              FacebookId = ctx.User.GetClaim "facebookId" |> Option.defaultValue "" }
        view (Some model) [] club user ctx

let post (club : Club) (user : Users.User option) form (ctx : HttpContext) =
    form
    |> function 
    | Ok form -> 
        validate [ <@ form.Fornavn @> >- [ isRequired
                                           minLength 2
                                           maxLength 50 ]
                   <@ form.Mellomnavn @> >- [ maxLength 50 ]
                   <@ form.Etternavn @> >- [ isRequired
                                             minLength 2
                                             maxLength 50 ] ]
        |> function 
        | Ok _ -> 
            let (ClubId clubId) = club.Id
            let db = ctx.Database
            db.MemberRequests.Add
                (MyTeam.Models.Domain.MemberRequest
                     (ClubId = clubId, FirstName = form.Fornavn, MiddleName = form.Mellomnavn, 
                      LastName = form.Etternavn, Email = ctx.User.Identity.Name, 
                      FacebookId = form.FacebookId)) |> ignore
            db.SaveChanges() |> ignore
            view None [] club user ctx
        | Error e -> view (Some form) e club user ctx
    | Error e -> failwith e
