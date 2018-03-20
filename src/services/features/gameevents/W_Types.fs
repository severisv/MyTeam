namespace MyTeam.Games.Events

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Domain.Members
open System

type GameEventType =
    | ``Mål`` = 0
    | ``Gult kort`` = 1
    | ``Rødt kort`` = 2

type GameEvent = {
    Id: Guid
    GameId: GameId
    Type: GameEventType
    PlayerId: PlayerId option
    AssistedById: PlayerId option
}

type Get = ClubId -> GameId -> Database -> Result<GameEvent list, Error>