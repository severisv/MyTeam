module MyTeam.Admin.Pages

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain.Members
open Shared.Features.Admin.AddPlayers
open MyTeam.Views

let index club user ctx =
    [ mtMain [] 
          [ block [] [ div [ _id "manage-players"
                             attr "data-statuses" (Enums.getValues<Status>() |> Json.serialize)
                             attr "data-roles" (Enums.getValues<Role>() |> Json.serialize) ] [] ] ]
      Admin.coachMenu ]
    |> layout club user (fun o -> { o with Title = "Administrer spillere" }) ctx
    |> OkResult

let invitePlayers club user ctx =
    [ Client.view clientView { Year = None }
      Admin.coachMenu ]
    |> layout club user (fun o -> { o with Title = "Inviter spillere" }) ctx
    |> OkResult
