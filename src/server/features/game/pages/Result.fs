module MyTeam.Games.Pages.Result

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open Shared.Domain.Members
open Shared.Components
open Client.Features.Games.EditEvents
open Client.Components
open Client.Components.AutoSync.Text


let view (club: Club) (user: User option) gameId (ctx: HttpContext) =

    let db = ctx.Database

    Queries.getGame db club.Id gameId
    |> function
        | None -> NotFound
        | Some game ->
            [ mtMain [] [
                  block [ _class "registerResult" ] [
                      a [ _href <| sprintf "/kamper/%O" game.Id
                          _class "pull-right"
                          _title "Vis kamp" ] [
                          !!(Icons.arrowLeft "")
                          span [ _class "hidden-xxs" ] [
                              encodedText "Tilbake"
                          ]
                      ]
                      div [ _class "game-header" ] [
                          div [ _class "registerResult-teamScore" ] [
                              encodedText game.HomeTeam
                              Client.comp
                                  AutoSync.Text.Element
                                  { Value = (toString game.HomeScore)
                                    Url = $"/api/games/{game.Id}/score/home"
                                    OnChange = None }


                          ]
                          div [ _class "hidden-xs" ] [
                              encodedText "-"
                          ]
                          div [ _class "registerResult-teamScore" ] [
                              Client.comp
                                  AutoSync.Text.Element
                                  { Value = (toString game.AwayScore)
                                    Url = $"/api/games/{game.Id}/score/away"
                                    OnChange = None }


                              encodedText game.AwayTeam
                          ]
                      ]
                      hr []
                      div [] [ Common.gameDetails game ]
                      (Client.clientView editGameEventsId { GameId = game.Id })
                  ]
              ]

              ]
            |> layout club user (fun o -> { o with Title = "Kamper" }) ctx
            |> OkResult
