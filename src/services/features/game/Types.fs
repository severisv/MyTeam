namespace MyTeam.Games

open System
open MyTeam
open Shared.Domain
open Shared.Domain.Members
open Shared.Domain.Events
open Shared.Components.Input
open Client.Games.SelectSquad

type Year = int

type GetRecentAttendance = Database -> TeamId -> TeamAttendance list
type GetSquad = Database -> GameId -> Member list
type SelectPlayer = Database -> ClubId -> GameId -> MemberId -> CheckboxPayload -> HttpResult<CheckboxPayload>
type GetGame = Database -> ClubId -> GameId -> Game option
type ListGameYears = Database -> TeamId -> Year list
type ListGames = Database -> TeamId -> Year -> Game list