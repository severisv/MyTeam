namespace Client.Events

open Shared.Domain
open System

type Attendee = {
    Id: Guid
    FirstName: string
    LastName: string
    UrlName: string
    IsAttending: bool
    Message: string
}

type Game = {
    Team: string
    Opponent: string
    Type: GameType
    SquadIsPublished: bool
    Squad: Attendee list
}


type Details =
    | Game of Game
    | Training

type Event = {
    Id: Guid
    Type: EventType
    DateTime: DateTime
    Location: string
    Description: string
    Details: Details
    TeamIds: Guid list
    Signups: Attendee list
 }

type Period = Upcoming | Previous


type Signup = {
    IsAttending: bool
}

