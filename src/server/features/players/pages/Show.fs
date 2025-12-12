module MyTeam.Players.Pages.Show

open Giraffe.ViewEngine
open MyTeam
open Shared
open Shared.Domain
open MyTeam.Views
open MyTeam.Views.BaseComponents
open Shared.Domain.Members
open Fable.React.Props
open System.Linq
open Shared.Components.Links
open Client.Features.Players
open Common
open Shared.Components


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
           Positions =
            !!!p.PositionsString
            |> Strings.split ','
            |> String.concat ", "
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

        let editPlayerUrl = Strings.toLower >> sprintf "/spillere/endre/%s"

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
                          img [
                              _src
                              <| Images.getMember ctx (fun o -> { o with Width = Some 1024 }) player.Image player.FacebookId
                          ]
                      ]
                      div [ _class "col-sm-6 col-md-6 col-lg-6 player-info" ] [
                          (match user with
                           | Some user when
                               (user.Id = player.Id
                                || user.IsInRole [ Trener; Admin ])
                               ->
                               !!(editAnchor [
                                   Class "edit-player-link "
                                   Href <| editPlayerUrl player.UrlName
                                  ])
                           | _ -> fragment [])

                          table [ _class "table" ] [
                              tbody [] [
                                  tableRow
                                      "Posisjon"
                                      (match player.Status with
                                       | PlayerStatus.Trener -> encodedText "Trener"
                                       | _ -> encodedText player.Positions)
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
                                          (a [ _href <| sprintf "mailto:%s" player.Email ] [
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
                          (Client.isomorphicView Stats.containerId Stats.element { PlayerId = player.Id })
                          div [ _style "margin-top: 1.5rem;" ] [
                              a [ _class "link-blue"
                                  _style "margin-left: 0.5em;"
                                  _href
                                  <| sprintf "/spillere/vis/%s/innsikt" player.UrlName ] [
                                  !!(Icons.lightbulb "")
                                  encodedText " Innsikt"
                              ]
                          ]
                      ]
                  ]
              ]
          ]
          sidebar player.Status players player.UrlName

          ]
        |> layout club user (fun o -> { o with Title = player.FullName }) ctx)
