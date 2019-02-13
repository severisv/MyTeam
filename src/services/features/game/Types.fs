namespace MyTeam.Games

open System
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open Shared.Domain.Events
open Shared.Components.Input
open Shared.Features.Games.SelectSquad

type Outcome = 
    | Seier
    | Tap
    | Uavgjort

type Game = {
    Id: Guid
    GamePlanIsPublished: bool
    IsHomeTeam:bool
    HomeTeam: string
    AwayTeam: string
    HomeScore: int option
    AwayScore: int option
    DateTime: DateTime
    Location: string
    Type: GameType
    MatchReportName: string option
} with
    member g.LocationShort = g.Location |> replace " kunstgress" ""
    member g.Outcome = 
            match (g.HomeScore, g.AwayScore) with
            | (Some homeScore, Some awayScore) ->                
                homeScore - awayScore
                |> fun score ->
                    if score > 0 then g.IsHomeTeam =? (Seier, Tap)
                    else if score < 0 then (not g.IsHomeTeam) =? (Seier, Tap)
                    else Uavgjort
                |> Some                
                        
            | _ -> None            

type Year = int

type GetRecentAttendance = Database -> TeamId -> TeamAttendance list
type GetSquad = Database -> GameId -> Member list
type SelectPlayer = Database -> ClubId -> GameId -> MemberId -> CheckboxPayload -> HttpResult<CheckboxPayload>
type PublishSquad = Database -> ClubId -> GameId -> HttpResult<unit>

type GetGame = Database -> ClubId -> GameId -> Game option
type ListGameYears = Database -> TeamId -> Year list
type ListGames = Database -> TeamId -> Year -> Game list