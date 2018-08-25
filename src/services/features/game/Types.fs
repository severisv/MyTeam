namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Events
open MyTeam.Domain.Members
open System
open MyTeam.Ajax
open MyTeam.Shared.Domain


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

type GetSquad = Database -> GameId -> Member list
type SelectPlayer = ClubId -> GameId * MemberId -> Database -> CheckboxPayload -> HttpResult<unit>

type GetGame = Database -> GameId -> Game option
type ListGameYears = Database -> TeamId -> Year list
type ListGames = Database -> TeamId -> Year -> Game list