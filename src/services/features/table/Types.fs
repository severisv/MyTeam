namespace MyTeam.Table

open MyTeam
open MyTeam.Domain

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


type Table = {
    Rows: TableRow list
    UpdatedDate: System.DateTime
    Title: string
}

type Year = int
type GetYears = Database -> TeamId -> Year list
type GetTable = Database -> TeamId -> Year -> Table option

