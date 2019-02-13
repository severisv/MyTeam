module MyTeam.Admin.Pages

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain.Members
open Shared.Features.Admin.AddPlayers
open MyTeam.Views
open Shared.Domain

let index club user ctx =
    [ mtMain [] 
          [ block [] [ div [ _id "manage-players"
                             attr "data-statuses" (Enums.getValues<Status>() |> Json.serialize)
                             attr "data-roles" (Enums.getValues<Role>() |> Json.serialize) ] [] ] ]
      Admin.coachMenu ]
    |> layout club user (fun o -> { o with Title = "Administrer spillere" }) ctx
    |> OkResult

let invitePlayers club user (ctx: HttpContext) =
    
    let db = ctx.Database
    let (ClubId clubId) = club.Id
    
    let memberRequests =
        query {
            for request in db.MemberRequests do
            where (request.ClubId = clubId)
            select (request.FirstName, request.MiddleName, request.LastName, request.Email, request.FacebookId)
        }
        |> Seq.toList
        |> List.map (fun (fornavn, mellomnavn, etternavn, epost, facebookId) ->
            {
                Fornavn = fornavn
                Etternavn = etternavn
                Mellomnavn = mellomnavn
                ``E-postadresse`` = epost
                FacebookId = facebookId
            }
            )
    
    [
      mtMain [_class "mt-main--narrow"] [
        Client.view clientView { MemberRequests = memberRequests; ImageOptions = Images.getOptions ctx }
      ]
      Admin.coachMenu ]
    |> layout club user (fun o -> { o with Title = "Inviter spillere" }) ctx
    |> OkResult
