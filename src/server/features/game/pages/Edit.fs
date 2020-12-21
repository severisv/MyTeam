module MyTeam.Games.Pages.Edit

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Games
open Shared.Domain
open MyTeam.Views
open System


let view (club: Club) user gameId (ctx: HttpContext) =

    Queries.getGame ctx.Database club.Id gameId
    |> function
    | Some game ->
        [ mtMain [ _class "mt-main--narrow" ]
              [ block []
                    [ Client.view2 Client.Features.Games.Form.containerId Client.Features.Games.Form.element
                          { Teams = club.Teams
                            GameTypes = Enums.getValues<GameType> ()
                            Game = Some game } ] ] ]
        |> layout club user (fun o -> { o with Title = "Endre kamp" }) ctx
        |> OkResult
    | None -> NotFound
