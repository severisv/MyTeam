module MyTeam.Attendance.Api

open MyTeam.Attendance
open MyTeam
open MyTeam.Models
open Shared.Domain
open MyTeam.Models.Domain
open Shared.Components.Input
open System
open System.Linq

let confirmAttendance =

    fun clubId (eventId, playerId) (ctx : HttpContext) (model: CheckboxPayload) ->
        let db = ctx.Database
        let (ClubId clubId) = clubId

        query {
            for e in db.Events do
            where (e.ClubId = clubId && e.Id = eventId)
            select (e.Id)
        }
        |> Seq.tryHead
        |> function
            | None -> Unauthorized
            | Some eventId ->
                db.EventAttendances.Where (fun e -> e.EventId = eventId && e.MemberId = playerId)
                |> Seq.tryHead
                |> function
                    | Some attendance ->
                        attendance.DidAttend <- model.value
                        db.EventAttendances.Attach(attendance) |> ignore
                    | None ->
                        db.EventAttendances.Add (
                                EventAttendance (
                                    Id = Guid.NewGuid(),
                                    EventId = eventId,
                                    DidAttend = model.value,
                                    IsAttending = Nullable false,
                                    MemberId = playerId)
                                               )
                        |> ignore
                db.SaveChanges() |> ignore
                OkResult()


let registerVictory =
    fun clubId (eventId, playerId) (ctx : HttpContext) model ->
        let db = ctx.Database

        let (ClubId clubId) = clubId

        query {
            for e in db.Events do
            where (e.ClubId = clubId && e.Id = eventId)
            select (e.Id)
        }
        |> Seq.tryHead
        |> function
            | None -> Unauthorized
            | Some eventId ->
                db.EventAttendances.Where(fun e -> e.EventId = eventId && e.MemberId = playerId)
                |> Seq.tryHead
                |> function
                    | Some attendance ->
                        attendance.WonTraining <- model.value
                        db.EventAttendances.Attach(attendance) |> ignore
                    | None ->
                        db.EventAttendances.Add (
                                EventAttendance (
                                    Id = Guid.NewGuid(),
                                    EventId = eventId,
                                    WonTraining = model.value,
                                    IsAttending = Nullable false,
                                    MemberId = playerId)
                                               )
                        |> ignore
                db.SaveChanges() |> ignore
                OkResult()

