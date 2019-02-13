module MyTeam.Events.Api

open MyTeam
open Shared
open Shared.Domain
open MyTeam.Models
open MyTeam.Models.Domain
open Shared.Components.Input

type EventId = Guid

type Event = {
    Id: EventId
    Description: string
}


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
