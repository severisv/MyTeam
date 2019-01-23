module MyTeam.Members.Pages.RequestAccess


open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open MyTeam.Members



let view (club: Club) (user: Users.User option) (ctx: HttpContext) =

    let db = ctx.Database

    user |> function
    | Some _ -> Redirect "/intern"
    | None ->
        [
            mtMain [] [
                block [_class "text-center"] 
                    (if not ctx.User.Identity.IsAuthenticated then
                        [
                            h4 [] [encodedText "Velkommen!"]
                            p [] [encodedText <| sprintf "Her kan du be om spillertilgang til %s." club.ShortName]
                            p [] [encodedText "Først må du "
                                  a [_href "konto/innlogging?returnUrl=/registrer"] [encodedText "logge inn"]
                                  encodedText "."  
                                ]
                        ]
                     else                     
                        [
                            p [] [encodedText "Registrer navnet ditt her, så får du en tilbakemelding så fort forespørselen er godkjent av noen i støtteapparatet." ]
                        ]
                    )
            ]
        ]
        |> layout club user (fun o -> { o with Title = "Be om tilgang" }) ctx
        |> OkResult
