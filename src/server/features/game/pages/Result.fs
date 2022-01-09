module MyTeam.Games.Pages.Result

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain
open MyTeam.Games
open MyTeam.Views
open Shared.Domain.Members
open Shared.Components
open Client.Features.Games.EditEvents


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
                          span [ _class "registerResult-teamScore" ] [
                              encodedText game.HomeTeam
                              input [
                                  _type "tel"
                                  _class "registerResult-score ajax-update"
                                  attr "data-href"
                                  <| sprintf "/api/games/%O/score/home" game.Id
                                  _value (toString game.HomeScore)
                              ]
                          ]
                          span [ _class "hidden-xs" ] [
                              encodedText "-"
                          ]
                          span [ _class "registerResult-teamScore" ] [
                              input [
                                  _type "tel"
                                  _class "registerResult-score ajax-update"
                                  attr "data-href"
                                  <| sprintf "/api/games/%O/score/away" game.Id
                                  _value (toString game.AwayScore)
                              ]
                              encodedText game.AwayTeam
                          ]
                      ]
                      hr []
                      div [] [ Common.gameDetails game ]
                      (Client.comp editGameEventsId { GameId = game.Id })
                      hr []
                      div [ _id "registerResult-addEvent"
                            attr "data-game-id" (string game.Id) ] []
                  ]
              ]

             ]
            |> layout club user (fun o -> { o with Title = "Kamper" }) ctx
            |> OkResult
