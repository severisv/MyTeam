module Shared.Features.Games.GamePlan

open System
open Shared
open Shared.Domain.Members
open Shared.Image

type ElleverFormation = 
    FourFourTwo | FourThreeThree
    override this.ToString() =
        match this with
        | FourFourTwo -> "4-4-2"
        | FourThreeThree -> "4-3-3"

type SjuerFormation = 
    ThreeTwoOne | TwoThreeOne
    override this.ToString() =
        match this with
        | ThreeTwoOne -> "3-2-1"
        | TwoThreeOne -> "2-3-1"
        
type Formations =
    | Sjuer of SjuerFormation
    | Ellever of ElleverFormation
    override this.ToString() =
        match this with
        | Sjuer v -> string v
        | Ellever v -> string v


type Model = {
    GameId: Guid
    Team: string
    Opponent: string
    GamePlanIsPublished: bool
    GamePlan: string option
    Players: Member list
    ImageOptions: CloudinaryOptions
    Formation: Formations
}

let clientView = "gameplan"
let modelAttribute = "model"