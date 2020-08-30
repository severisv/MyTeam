module MyTeam.Players.Pages.Show

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Components
open Shared.Components.Nav
open System
open Shared.Domain.Members
open Fable.React.Props
open System.Linq
open Shared.Components.Links


let view (club: Club) (user: User option) urlName (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let urlName = urlName |> Strings.toLower

    (db.Members.Where(fun m -> m.ClubId = clubId && m.UrlName = urlName))
    |> Common.Features.Members.selectMembers
    |> Seq.toList
    |> List.tryHead
    |> HttpResult.fromOption (fun player ->


        let players =
            let statusInt = (player.Status |> PlayerStatus.toInt)
            (db.Members.Where(fun m -> m.ClubId = clubId && m.Status = statusInt))
            |> Common.Features.Members.selectMembers
            |> Seq.toList


        let listPlayersUrl =
            function
            | Aktiv -> sprintf "/spillere"
            | status -> sprintf "/spillere/%O" status

        let showPlayerUrl =
            Strings.toLower >> sprintf "/spillere/vis/%s"

        let editPlayerUrl =
            Strings.toLower
            >> sprintf "/spillere/vis/%s/endre"

        [ mtMain [] [
            block [] [
                div [ _class "row" ] [
                    div [ _class "col-sm-6 col-md-6 col-lg-6 picture-frame" ] [
                        img
                            [ _src
                              <| Images.getMember ctx (fun o -> { o with Width = Some 400 }) player.Image
                                     player.FacebookId ]
                    ]
                    div [ _class "col-sm-6 col-md-6 col-lg-6 player-info" ] [
                        (match user with
                         | Some user when (user.Id = player.Id
                                           || user.IsInRole [ Trener; Admin ]) ->
                             !!(editAnchor [ Class "edit-player-link "
                                             Href <| editPlayerUrl player.UrlName ])
                         | _ -> fragment [])

                        Giraffe.GiraffeViewEngine.table [ _class "table" ] [
                            tbody [] [
                                tr [] [
                                    td [] [
                                        label [] [ encodedText "Posisjon" ]
                                    ]
                                    td [ _class "playerStats--value" ] [
                                        encodedText "Midtstopper"
                                    ]
                                ]
                                tr [] [
                                    td [] [
                                        label [] [
                                            encodedText "Signerte for klubben"
                                        ]
                                    ]
                                    td [ _class "playerStats--value" ] [
                                        encodedText "2016"
                                    ]
                                ]
                                tr [] [
                                    td [] [
                                        label [] [ encodedText "Født" ]
                                    ]
                                    td [ _class "playerStats--value" ] [
                                        encodedText "07.04.1995"
                                    ]
                                ]
                                tr [] [
                                    td [] [
                                        label [] [ encodedText "E-post" ]
                                    ]
                                    td [ _class "playerStats--value" ] [
                                        a [ _href "mailto:einar.solheim@weika.no" ] [
                                            encodedText "einar.solheim@weika.no"
                                        ]
                                    ]
                                ]
                                tr [] [
                                    td [] [
                                        label [] [ encodedText "Telefon" ]
                                    ]
                                    td [ _class "playerStats--value" ] [
                                        a [ _href "tel:94863773" ] [
                                            encodedText "94863773"
                                        ]
                                    ]
                                ]
                            ]
                        ]
                        div [ _class "ajax-load"
                              _href "/spillere/stats?playerId=395b37b4-e957-4543-80b5-aad42108523d" ] [
                            div [ _class "playerStats" ] []
                        ]
                    ]
                ]
            ]
          ]

          sidebar [] [
              block [] [
                  !!(navList
                      { Header = "Undermeny"
                        Items =
                            [ { Text = [ (string >> Fable.React.Helpers.str) "Aktive spillere" ]
                                Url = listPlayersUrl Aktiv }
                              { Text = [ (string >> Fable.React.Helpers.str) "Hall of Fame" ]
                                Url = listPlayersUrl Veteran } ]
                        Footer = None
                        IsSelected = never })
              ]
              block [] [
                  !!(navList
                      { Header =
                            match player.Status with
                            | Veteran -> "Hall of Fame"
                            | status -> sprintf "%Oe Spillere" status
                        Items =
                            players
                            |> List.map (fun player ->
                                { Text = [ (string >> Fable.React.Helpers.str) player.Name ]
                                  Url = showPlayerUrl player.UrlName })
                        Footer = None
                        IsSelected = (=) (showPlayerUrl player.UrlName) })
              ]
          ] ]
        |> layout club user (fun o -> { o with Title = player.FullName }) ctx)
