module MyTeam.Players.Pages.List

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open Shared.Components.Nav
open Shared.Domain.Members
open System.Linq


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
        | status -> sprintf "%Oe Spillere" status

    let players =
        let statusInt = (status |> PlayerStatus.toInt)
        (db.Members.Where(fun m -> m.ClubId = clubId && m.Status = statusInt))
        |> Common.Features.Members.selectMembers
        |> Seq.toList

    let listPlayersUrl =
        function
        | Aktiv -> sprintf "/spillere"
        | status -> sprintf "/spillere/%O" status

    let showPlayerUrl =
        Strings.toLower >> sprintf "/spillere/vis/%s"

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
                                 img
                                     [ _src
                                       <| Images.getMember ctx (fun o ->
                                              { o with
                                                    Width = Some 100
                                                    Height = Some 100 }) player.Image player.FacebookId ]
                             ]
                             p [] [ encodedText player.Name ]
                         ]
                     ]))
        ]
      ]

      sidebar [] [
          block [] [
              !!(navList
                  { Header = "Spillerkategori"
                    Items =
                        [ { Text = [ (string >> Fable.React.Helpers.str) "Aktive spillere" ]
                            Url = listPlayersUrl Aktiv }
                          { Text = [ (string >> Fable.React.Helpers.str) "Hall of Fame" ]
                            Url = listPlayersUrl Veteran } ]
                    Footer = None
                    IsSelected = (=) (listPlayersUrl status) })
          ]
          block [] [
              !!(navList
                  { Header = pageHeader
                    Items =
                        players
                        |> List.map (fun player ->
                            { Text = [ (string >> Fable.React.Helpers.str) player.Name ]
                              Url = showPlayerUrl player.UrlName })
                    Footer = None
                    IsSelected = never })
          ]
      ] ]
    |> layout club user (fun o -> { o with Title = "Aktive spillere" }) ctx
    |> OkResult
