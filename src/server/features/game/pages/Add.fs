module MyTeam.Games.Pages.Add

open Giraffe.ViewEngine
open MyTeam
open Shared.Domain
open MyTeam.Views
open System


let view (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database

    [ mtMain [ _class "mt-main--narrow" ] [
          block [] [
              Client.isomorphicView
                  Client.Features.Games.Form.containerId
                  Client.Features.Games.Form.element
                  { Teams = club.Teams
                    GameTypes = Enums.getValues<GameType> ()
                    Game = None }
          ]
      ] ]
    |> layout club user (fun o -> { o with Title = "Ny kamp" }) ctx
    |> OkResult
