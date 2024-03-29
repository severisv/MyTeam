namespace Client.Events

open Client.GamePlan.View
open Shared.Domain
open System

type Attendee =
    { Id: Guid
      FirstName: string
      LastName: string
      UrlName: string
      IsAttending: bool option
      DidAttend: bool
      Message: string }

type Game =
    { Team: string
      Opponent: string
      Type: GameType
      SquadIsPublished: bool
      GamePlanIsPublished: bool
      Squad: Attendee list }


type Details =
    | Game of Game
    | Training



type Event =
    { Id: Guid
      Type: EventType
      DateTime: DateTime
      Location: string
      Description: string
      Details: Details
      TeamIds: Guid list
      Signups: Attendee list }



type SubPeriod =
    | NearFuture
    | Rest

type YearPeriod = int option

type Period =
    | Upcoming of SubPeriod
    | Previous of YearPeriod

type Signup = { IsAttending: bool }


module Event =
    let allowedSignupDays = 14.0
    let allowedSignoffHours = 2.0

    let signupHasOpened e =
        let isTreningskamp =
            match e.Details with
            | Game game -> game.Type = GameType.Treningskamp
            | _ -> false

        isTreningskamp
        || e.DateTime <= DateTime.Now.AddDays 14.0
