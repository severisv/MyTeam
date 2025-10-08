module MyTeam.Games.Pages.Show

open Giraffe.ViewEngine
open Fable.React.Props
open MyTeam
open Shared
open Server
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open Shared.Domain.Members
open System
open Shared.Components
open Shared.Components.Links
open MyTeam.Views.BaseComponents
open Client.Features.Games.ListEvents


let view (club: Club) (user: User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getGame db club.Id gameId
    |> function
        | None -> NotFound
        | Some game ->

            let matchReport =
                game.MatchReportName
                |> Option.map (Common.News.Queries.getArticle db club.Id)
                |> Option.defaultValue None

            let gameHasPassed = game.DateTime < DateTime.Now

            [ mtMain [] [

                  div [ _class "mt-container" ] [
                      user
                      => fun user ->
                          if
                              user.IsInRole [
                                  Role.Admin
                                  Role.Trener
                              ] then
                              !!(editAnchor [
                                  Href <| sprintf "/kamper/%O/endre" game.Id
                                 ])
                          else
                              empty

                      user
                      => fun user ->
                          if user.IsInRole [
                              Role.Admin
                              Role.Trener
                              Role.Skribent
                             ]
                             && gameHasPassed then
                              a [ _href <| sprintf "/kamper/%O/resultat" game.Id
                                  _class "edit-link pull-right" ] [
                                  !!(Icons.ballInGoal "Registrer resultat")
                              ]
                          else
                              empty

                      user
                      => fun user ->
                          if user.IsInRole [ Role.Trener ]
                             || user.UserId = "severin@sverdvik.no"
                             || game.GamePlanIsPublished then
                              a [ _href <| sprintf "/kamper/%O/bytteplan" game.Id
                                  _class "edit-link registerSquad-gameplan-link pull-right" ] [
                                  !!Icons.gamePlan
                              ]
                          else
                              empty

                      h3 [ _class "game-header hidden-xs" ] [
                          span [ _class "game-score-team"
                                 _style "text-align:right;" ] [
                              encodedText game.HomeTeam
                          ]
                          span [ _class "game-score" ] [
                              encodedText
                              <| sprintf
                                  "%s - %s"
                                  (game.HomeScore
                                   |> Option.map string
                                   |> Option.defaultValue "")
                                  (game.AwayScore
                                   |> Option.map string
                                   |> Option.defaultValue "")
                          ]
                          span [ _class "game-score-team"
                                 _style "text-align:left;" ] [
                              encodedText game.AwayTeam
                          ]
                      ]

                      HtmlElements.table [ _class "game-scoreTable visible-xs" ] [
                          tr [] [
                              td [] [ encodedText game.HomeTeam ]
                              td [ _class "game-scoreTable--score" ] [
                                  game.HomeScore
                                  |> Option.map (string >> encodedText)
                                  |> Option.defaultValue empty
                              ]
                              tr [] [
                                  td [] [ encodedText game.AwayTeam ]
                                  td [ _class "game-scoreTable--score" ] [
                                      game.AwayScore
                                      |> Option.map (string >> encodedText)
                                      |> Option.defaultValue empty
                                  ]
                              ]
                          ]
                      ]
                      hr []
                      div [] [ Common.gameDetails game ]
                      gameHasPassed
                      =? (Client.clientView listGameEventsId { GameId = game.Id },

                          empty)
                  ]
                  matchReport
                  => fun matchReport ->
                      block [ _class "u-fade-in-on-enter" ] [
                          h2 [ _class "news-matchReport" ] [
                              encodedText "Kamprapport"
                          ]
                          hr []
                          Common.News.Components.showArticle ctx user matchReport None
                      ]
              ] ]
            |> layout club user (fun o -> { o with Title = "Kampdetaljer" }) ctx
            |> OkResult
