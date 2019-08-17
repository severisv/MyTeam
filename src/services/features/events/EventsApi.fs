module MyTeam.Events.Api

open System
open MyTeam
open Shared.Domain
open MyTeam.Models
open MyTeam.Models.Domain
open Shared.Components.Input
open Shared.Domain.Members

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


let internal updateAttendance clubId (user: User) eventId (ctx : HttpContext) updateFn =
    let db = ctx.Database
    let (ClubId clubId) = clubId

    query {
        for e in db.Events do
            where (e.Id = eventId)
            select e.ClubId }
    |> Seq.head
    |> fun id ->
        
        if id <> clubId then 
            Unauthorized
    
        else
            query { for ea in db.EventAttendances do
                        where (ea.MemberId = user.Id && ea.EventId = eventId)
                        select ea }
            |> Seq.tryHead
            |> function
            | Some ea ->
                ea
            | None -> EventAttendance(MemberId = user.Id,
                                      EventId = eventId)
            |> fun ea ->
                updateFn ea
                db.EventAttendances.Attach ea |> ignore
                db.SaveChanges() |> ignore
                OkResult None
                

let signup clubId (user: User) eventId (ctx : HttpContext) (model: Client.Events.List.Signup) =
    updateAttendance clubId user eventId ctx (fun ea ->
                                                    ea.IsAttending <- Nullable model.IsAttending)

let signupMessage clubId (user: User) eventId (ctx : HttpContext) (model: StringPayload) =
    updateAttendance clubId user eventId ctx (fun ea ->
                                                    ea.SignupMessage <- model.Value 
                                             )