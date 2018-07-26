namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Domain.Members
open System
open MyTeam.Ajax
open MyTeam.Shared.Domain


type Game = {
    Id: Guid
    GamePlanIsPublished: bool
    HomeTeam: string
    AwayTeam: string
    HomeScore: int option
    AwayScore: int option
    DateTime: DateTime
    Location: string
    Type: GameType
    MatchReportName: string option
}


type GetSquad = Database -> GameId -> Member list
type SelectPlayer = ClubId -> GameId * MemberId -> Database -> CheckboxPayload -> HttpResult<unit>

type GetGame = Database -> GameId -> Game option