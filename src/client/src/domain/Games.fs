namespace Shared.Domain

open System
open Shared.Domain

type Outcome =
    | Seier
    | Tap
    | Uavgjort

type GameTeam = { Id: TeamId; Name: string }

type Game =
    { Id: Guid
      Team: GameTeam
      GamePlanIsPublished: bool
      IsHomeTeam: bool
      Opponent: string
      HomeScore: int option
      AwayScore: int option
      DateTime: DateTime
      Location: string
      Description: string
      Type: GameType
      MatchReportName: string option }
    member g.HomeTeam = if g.IsHomeTeam then g.Team.Name else g.Opponent
    member g.AwayTeam = if g.IsHomeTeam then g.Opponent else g.Team.Name
    member g.LocationShort = g.Location |> Shared.Strings.replace " kunstgress" ""
    member g.Name = sprintf "%s - %s" g.HomeTeam g.AwayTeam

    member g.Outcome =
        match (g.HomeScore, g.AwayScore) with
        | (Some homeScore, Some awayScore) ->
            homeScore
            - awayScore
            |> fun score ->
                match (g.IsHomeTeam) with
                | true when score > 0 -> Seier
                | false when score < 0 -> Seier
                | true when score < 0 -> Tap
                | false when score > 0 -> Tap
                | _ -> Uavgjort
            |> Some

        | _ -> None
