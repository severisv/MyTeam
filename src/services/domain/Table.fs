module MyTeam.Domain.Table
open MyTeam

type TableString = string

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

let fromJson obj = 
    Json.fableDeserialize<TableRow list> obj
    |> function 
    | Ok value -> value 
    | Error e -> failwith e

let fromString (table: TableString) =
        table.Split('|')
        |> Array.map (fun row ->
                        row.Split(';')
                        |> fun row ->
                                if row.Length > 6 then 
                                        Some ({
                                                Position = row.[0] |> Number.parse
                                                Team = row.[1]
                                                Wins = row.[2] |> Number.parse
                                                Draws = row.[3] |> Number.parse
                                                Losses = row.[4] |> Number.parse
                                                GoalsFor = row.[5] |> Number.parse
                                                GoalsAgainst = row.[6] |> Number.parse
                                                Points = row.[7] |> Number.parse
                                        })
                                else None                                                                
                    )
        |> Array.toList
        |> List.choose id
                   
