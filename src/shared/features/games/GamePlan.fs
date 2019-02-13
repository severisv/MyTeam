module Shared.Features.Games.GamePlan

open System
open Shared
open Shared.Domain.Members
open Shared.Image


type Model = {
    GameId: Guid
    Team: string
    Opponent: string
    GamePlanIsPublished: bool
    GamePlan: string option
    Players: Member list
    ImageOptions: CloudinaryOptions
}

let clientView = "gameplan"
let modelAttribute = "model"