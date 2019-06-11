module MyTeam.Events.Api

open MyTeam
open Shared.Domain
open MyTeam.Models
open Shared.Components.Input

type EventId = Guid

type Event = {
    Id: EventId
    Description: string
}

let setDescription clubId eventId (ctx : HttpContext) (model: StringPayload) =
    let db = ctx.Database
    let (ClubId clubId) = clubId

    query {
        for e in db.Events do
            where (e.Id = eventId)
            select e }
    |> Seq.head
    |> fun event ->
        
        if event.ClubId <> clubId then 
            Unauthorized
    
        else
            event.Description <- model.Value
            db.SaveChanges() |> ignore
            OkResult None
