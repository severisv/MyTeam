namespace MyTeam.Table

open MyTeam
open Shared
open Shared.Domain

type TableRow = {
        Team: string
        Position: int
        Points: int
        GoalsFor: int
        GoalsAgainst: int
        Wins: int
        Draws: int
        Losses: int
    } with 
        member row.Games = row.Wins + row.Draws + row.Losses
        member row.GoalDifference = sprintf "%i - %i" row.GoalsFor row.GoalsAgainst


type LeagueTable = {
    Rows: TableRow list
    UpdatedDate: System.DateTime
    Title: string
    AutoUpdate: bool
    SourceUrl: string
    AutoUpdateFixtures: bool
    FixtureSourceUrl: string
}

type Year = int
type GetYears = Database -> TeamId -> Year list
type GetTable = Database -> TeamId -> Year -> LeagueTable option

