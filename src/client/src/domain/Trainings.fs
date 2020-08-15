namespace Shared.Domain

open System
open Shared.Domain
open Shared

type TrainingId = Guid

type Training =
    { Id: TrainingId
      Teams: TeamId list
      DateTime: DateTime
      Location: string
      Description: string }
    member t.Name = sprintf "%s %s" t.Location (Date.format t.DateTime)
