module MyTeam.Members.Pages.RequestAccess

open Fable.Helpers.React.Props
open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Components.Buttons
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Members
open MyTeam.Shared.Components.Forms
open MyTeam.Views

let view (club : Club) (user : Users.User option) (ctx : HttpContext) =
    let db = ctx.Database
    user
    |> function 
    | Some _ -> Redirect "/intern"
    | None -> 
        [ mtMain [] 
              [ block [] 
                    (if not ctx.User.Identity.IsAuthenticated then 
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
                     else 
                         [ p [ _class "text-center" ] 
                               [ encodedText 
                                     "Registrer navnet ditt her, så får du en tilbakemelding så fort forespørselen er godkjent av noen i støtteapparatet." ]
                           br []
                           
                           !!(form [ Horizontal 2
                                     Action "/blimed"
                                     Method "POST" ] 
                                  [ formRow [ Horizontal 2 ] [ Fable.Helpers.React.str "Fornavn" ] 
                                        [ textInput [ Name "firstname" ] ]
                                    
                                    formRow [ Horizontal 2 ] [ Fable.Helpers.React.str "Etternavn" ] 
                                        [ textInput [ Name "lastname" ] ]
                                    
                                    formRow [ Horizontal 2 ] [] 
                                        [ btn [ Primary ] [ Fable.Helpers.React.str "Send" ] ] ]) ]) ] ]
        |> layout club user (fun o -> { o with Title = "Be om tilgang" }) ctx
        |> OkResult
