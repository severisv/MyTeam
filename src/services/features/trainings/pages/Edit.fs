module MyTeam.Trainings.Pages.Edit

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Trainings
open Shared.Domain
open MyTeam.Views
open System

let view (club: Club) user gameId (ctx: HttpContext) =

    match Queries.getTraining ctx.Database club.Id gameId with
    | Some game ->
        [ mtMain [ _class "mt-main--narrow" ]
              [ block []
                    [ Client.view2 Client.Features.Trainings.Form.containerId Client.Features.Trainings.Form.element
                          { Teams = club.Teams
                            Training = Some game } ] ] ]
        |> layout club user (fun o -> { o with Title = "Endre trening" }) ctx
        |> OkResult
    | None -> NotFound
