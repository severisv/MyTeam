namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System

type GameId = Guid


type GetSquad = Database -> ClubId -> GameId -> Member list
type SelectPlayer = ClubId -> GameId -> MemberId -> bool -> Database -> Result<unit,Error>