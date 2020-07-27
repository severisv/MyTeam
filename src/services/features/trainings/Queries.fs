module MyTeam.Trainings.Queries

open MyTeam
open Shared
open Shared.Domain
open Shared.Strings
open MyTeam.Common.Features.Members
open System.Linq
open System
open Client.Games.SelectSquad

let getTraining : Database -> ClubId -> TrainingId -> Training option =
    fun db clubId trainingId ->
        let (ClubId clubId) = clubId
        query {
            for training in db.Events do
                where
                    (training.Id = trainingId
                     && training.ClubId = clubId)
                select (training.DateTime, training.Location, training.Description, training.EventTeams.Select(fun e -> e.TeamId))
        }
        |> Seq.map (fun (dateTime, location, description, teamIds) ->
            { Id = trainingId
              Teams = teamIds |> Seq.toList
              DateTime = dateTime
              Location = !!location
              Description = !!description })
        |> Seq.tryHead
