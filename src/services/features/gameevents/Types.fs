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

type Get = ClubId -> GameId -> Database -> HttpResult<GameEvent list>
type Add = ClubId -> GameId -> Database -> GameEvent -> HttpResult<GameEvent>
type Delete = ClubId -> (GameId * GameEventId) -> Database -> HttpResult<unit>