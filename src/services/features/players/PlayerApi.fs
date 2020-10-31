module MyTeam.Players.Api

open Shared.Domain
open MyTeam
open System.Linq
open Client.Features.Players.Form
open System


let internal updatePlayer clubId playerId (db: Database) updateFn =
    db.Members.Where(fun t -> t.Id = playerId)
    |> Seq.tryHead
    |> function
    | Some player when (ClubId player.ClubId) <> clubId -> Unauthorized
    | Some player ->
        updateFn player
        db.SaveChanges() |> OkResult
    | None -> NotFound




let update (club: Club) trainingId (ctx: HttpContext) (model: EditPlayer) =
    updatePlayer club.Id trainingId ctx.Database (fun player ->

        player.BirthDate <- Nullable(model.BirthDate.ToLocalTime().Date)
        player.StartDate <- Nullable(model.StartDate.ToLocalTime().Date)
        player.PositionsString <- model.Positions |> String.concat ","
        player.ProfileIsConfirmed <- true
        player.Phone <- model.Phone
        player.ImageFull <- model.Image
        player.FirstName <- model.FirstName
        player.MiddleName <- model.MiddleName
        player.LastName <- model.LastName        
    )

    |> HttpResult.map (fun _ -> model)
