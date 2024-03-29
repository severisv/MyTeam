module MyTeam.Admin.Pages

open Client.Admin
open Client.Admin.AddPlayers
open Giraffe.ViewEngine
open MyTeam
open MyTeam.Views
open Shared
open Shared.Domain
open Shared.Domain.Members
open Shared.Components
open Client.Features.Admin.ManagePlayers

let internal coachMenu children =
    sidebar [] [
        block [] [
            ul
                [ _class "nav nav-list adminMenu" ]
                ([ li [ _class "nav-header" ] [
                       str "Admin"
                   ] ]
                 @ children)
        ]
    ]



let index (club: Club) user (ctx: HttpContext) =
    let members = Common.Features.Members.list (ctx.Database) club.Id

    [ mtMain [] [
          block [] [
              Client.clientView
                  containerId
                  { Teams = club.Teams |> List.sortBy (fun t -> t.ShortName)
                    Members = members
                    ImageOptions = Images.getOptions ctx }
          ]
      ]
      (coachMenu [
          li [] [
              a [ _href "/admin/spillerinvitasjon" ] [
                  !!(Icons.add "")
                  str "Legg til spiller"
              ]
          ]
       ]) ]
    |> layout club user (fun o -> { o with Title = "Administrer spillere" }) ctx
    |> OkResult

let invitePlayers (club: Club) user (ctx: HttpContext) =

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
            { Fornavn = Strings.defaultValue fornavn
              Etternavn = Strings.defaultValue etternavn
              Mellomnavn = Strings.defaultValue mellomnavn
              ``E-postadresse`` = Strings.defaultValue epost
              FacebookId = facebookId })

    [ mtMain [ _class "mt-main--narrow" ] [
          Client.isomorphicView
              clientView
              element
              { MemberRequests = memberRequests
                ImageOptions = Images.getOptions ctx }
      ]
      (coachMenu [
          li [] [
              a [ _href "/admin" ] [
                  !!(Icons.users "")
                  str "Administrer spillere"
              ]
          ]
       ]) ]
    |> layout club user (fun o -> { o with Title = "Inviter spillere" }) ctx
    |> OkResult
