namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Domain.Members
open System
open MyTeam.Ajax


type GetSquad = Database -> ClubId -> GameId -> Member list
type SelectPlayer = ClubId -> GameId * MemberId -> Database -> CheckboxPayload -> Result<unit,Error>