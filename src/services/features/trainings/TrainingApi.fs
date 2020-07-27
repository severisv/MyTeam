module MyTeam.Trainings.Api

open Server
open Shared.Domain
open MyTeam
open Shared
open System
open System.Linq
open Giraffe
open Client.Features.Trainings.Form


let internal updateGame clubId trainingId (db: Database) updateFn =
    db.Events.Where(fun t -> t.Id = trainingId)
    |> Seq.tryHead
    |> function
    | Some training when (ClubId training.ClubId) <> clubId -> Unauthorized
    | Some training ->
        updateFn training
        db.SaveChanges() |> OkResult
    | None -> NotFound



let add (club: Club) (ctx: HttpContext) (model: AddOrUpdateTraining) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let training =
        Models.Domain.Event
            (ClubId = clubId,
             DateTime = model.Date.Date + model.Time,
             Location = model.Location,
             Type = Events.eventTypeToInt Trening,
             Description = model.Description,
             EventTeams = (model.Teams |> List.map(fun teamId -> Models.Domain.EventTeam(TeamId = teamId)) |> List.toArray)
             )

    db.Events.Add(training) |> ignore
    db.SaveChanges() |> ignore
    OkResult
        { model with
              Id = Some training.Id
              Date = training.DateTime }

let update (club: Club) trainingId (ctx: HttpContext) (model: AddOrUpdateTraining) =
    updateGame club.Id trainingId ctx.Database (fun game ->

        game.DateTime <- model.Date.Date + model.Time
        game.Location <- model.Location
        game.Description <- model.Description        

        let db = ctx.Database
        let eventTeams = 
            db.EventTeams.Where(fun e -> e.EventId = trainingId)
            |> Seq.toList

        eventTeams
        |> List.filter (fun e -> model.Teams |> List.contains e.TeamId |> not)
        |> List.iter (db.Remove >> ignore)    

        let addedTeams = 
            (model.Teams 
            |> List.filter (fun teamId -> eventTeams |> List.exists (fun t -> t.TeamId = teamId) |> not)
            |> List.map(fun teamId -> Models.Domain.EventTeam(TeamId = teamId, EventId = trainingId)) |> List.toArray)

        db.EventTeams.AddRange(addedTeams) |> ignore    

        )
        |> HttpResult.map (fun _ -> model)

        

let delete (club: Club) trainingId db =
    updateGame club.Id trainingId db (db.Remove >> ignore)
