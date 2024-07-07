module MyTeam.Players.Pages.List

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain
open MyTeam.Views
open Shared.Domain.Members
open System.Linq
open Common

let view (club: Club) user status (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let status =
        status
        |> Enums.tryFromString<PlayerStatus>
        |> function
            | Ok status -> status
            | Error e -> Aktiv

    let pageHeader =
        match status with
        | Veteran -> "Hall of Fame"
        | PlayerStatus.Trener -> "Trenere"
        | status -> sprintf "%Oe Spillere" status

    let players =
        let statusInt = (status |> PlayerStatus.toInt)

        (db.Members.Where(fun m -> m.ClubId = clubId && m.Status = statusInt))
        |> Common.Features.Members.selectMembers
        |> Seq.toList

    [ mtMain [] [
          block [] [
              div
                  [ _class "row" ]
                  (players
                   |> List.map (fun player ->
                       div [ _class "col-lg-3 col-md-4 col-xs-4 text-center mt-playerWrapper" ] [
                           a [ _class "mt-player-container"
                               _href <| showPlayerUrl player.UrlName ] [
                               div [ _class "image-container" ] [
                                   img [
                                       _src
                                       <| Images.getMember
                                           ctx
                                           (fun o ->
                                               { o with
                                                   Width = Some 250
                                                   Height = Some 250 })
                                           player.Image
                                           player.FacebookId
                                   ]
                               ]
                               p [] [ encodedText player.Name ]
                           ]
                       ]))
          ]
      ]

      sidebar status players "" ]
    |> layout club user (fun o -> { o with Title = pageHeader }) ctx
    |> OkResult
