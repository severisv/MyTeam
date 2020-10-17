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
open Client.Features.Players


let view (club: Club) (user: User option) urlName (ctx: HttpContext) =

    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let urlName = urlName |> Strings.toLower

    let (!!!) = Strings.defaultValue
    db.Members.Where(fun m -> m.ClubId = clubId && m.UrlName = urlName)
    |> Seq.toList
    |> List.map (fun p ->
        {| Id = p.Id
           FacebookId = !!!p.FacebookId
           FirstName = !!!p.FirstName
           MiddleName = !!!p.MiddleName
           LastName = !!!p.LastName
           FullName = sprintf "%s %s %s" p.FirstName p.MiddleName p.LastName
           UrlName = !!!p.UrlName
           Image = !!!p.ImageFull
           Status = PlayerStatus.fromInt p.Status
           Positions = !!!p.PositionsString |> Strings.split ',' |> String.concat ", "
           StartDate = p.StartDate |> Option.fromNullable
           BirthDate = p.BirthDate |> Option.fromNullable
           Email = !!!p.UserName
           Phone = !!!p.Phone |})
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
            // >> sprintf "/spillere/vis/%s/endre"
            >> sprintf "/spillere/endre?playerId=%O"

        let tableRow lbl value =
            tr [] [
                td [] [ label [] [ encodedText lbl ] ]
                td [ _class "playerStats--value" ] [
                    value
                ]
            ]

        [ mtMain [] [
            block [] [
                div [ _class "row" ] [
                    div [ _class "col-sm-6 col-md-6 col-lg-6 picture-frame" ] [
                        img [ _src
                              <| Images.getMember ctx (fun o -> { o with Width = Some 400 }) player.Image
                                     player.FacebookId ]
                    ]
                    div [ _class "col-sm-6 col-md-6 col-lg-6 player-info" ] [
                        (match user with
                         | Some user when (user.Id = player.Id
                                           || user.IsInRole [ Trener; Admin ]) ->
                             !!(editAnchor [ Class "edit-player-link "
                                             Href <| editPlayerUrl player.Id ])
                         | _ -> fragment [])

                        Giraffe.GiraffeViewEngine.table [ _class "table" ] [
                            tbody [] [
                                tableRow "Posisjon" (encodedText player.Positions)
                                tableRow
                                    "Signerte for klubben"
                                    (player.StartDate
                                     |> Option.map (fun b -> string b.Year)
                                     |> Option.defaultValue ""
                                     |> encodedText)
                                tableRow
                                    "Født"
                                    (player.BirthDate
                                     |> Option.map (fun b -> string b.Year)
                                     |> Option.defaultValue ""
                                     |> encodedText)

                                user
                                => fun _ ->
                                    tableRow
                                        "E-post"
                                        (a [ _href <| sprintf "mailto:%s" player.Email] [
                                            encodedText player.Email
                                         ])
                                user
                                => fun _ ->
                                    tableRow
                                        "Telefonnummer"
                                        (a [ _href <| sprintf "tel:%s" player.Phone ] [
                                            encodedText player.Phone
                                         ])
                            ]
                        ]
                        (Client.view2 Stats.containerId Stats.element { PlayerId = player.Id})
                    ]
                ]
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
