namespace MyTeam.Games.Events

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Domain.Members
open System

type GameEventId = Guid

type GameEventType =
    | ``Mål`` = 0
    | ``Gult kort`` = 1
    | ``Rødt kort`` = 2

[<CLIMutable>]
type GameEvent = {
    Id: GameEventId
    Type: GameEventType
    PlayerId: PlayerId option
    AssistedById: PlayerId option
}

type Get = ClubId -> GameId -> Database -> Result<GameEvent list, Error>
type Add = ClubId -> GameId -> Database -> GameEvent -> Result<GameEvent, Error>
type Delete = ClubId -> (GameId * GameEventId) -> Database -> Result<unit, Error>