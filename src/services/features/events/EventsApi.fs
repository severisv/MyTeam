module MyTeam.Events.Api

open MyTeam
open MyTeam.Domain
open MyTeam.Models
open MyTeam.Models.Domain
open MyTeam.Events
open Shared.Components.Input

let setDescription clubId eventId (ctx : HttpContext) (model: StringPayload) =
    let db = ctx.Database
    let (ClubId clubId) = clubId
    let event = db.Events
                |> Seq.filter(fun e -> e.Id = eventId)
                |> Seq.head

    if event.ClubId <> clubId then 
        Unauthorized

    else
        event.Description <- model.Value
        db.SaveChanges() |> ignore
        OkResult()
