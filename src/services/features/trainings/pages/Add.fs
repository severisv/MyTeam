module MyTeam.Trainings.Pages.Add

open Giraffe.GiraffeViewEngine
open MyTeam
open Shared.Domain
open MyTeam.Views
open System


let view (club: Club) user (ctx: HttpContext) =

    let db = ctx.Database

    [ mtMain [ _class "mt-main--narrow" ]
          [ block []
                [ Client.view2 Client.Features.Trainings.Form.containerId Client.Features.Trainings.Form.element
                      { Teams = club.Teams
                        Training = None } ] ]]
    |> layout club user (fun o -> { o with Title = "Ny trening" }) ctx
    |> OkResult
