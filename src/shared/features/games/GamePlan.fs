module Shared.Features.Games.GamePlan

open System
open MyTeam
open MyTeam.Domain.Members
open MyTeam.Image


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