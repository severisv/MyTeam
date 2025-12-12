namespace MyTeam.Players

open System
open System.Linq
open MyTeam
open MyTeam.Models.Enums
open MyTeam.Stats
open Shared.Domain

type TeamInsights =
    { TeamId: Guid
      TeamName: string
      Games: int
      Wins: int
      WinRate: float
      CleanSheets: int
      GoalsPerGame: float
      AssistsPerGame: float
      AvgGoalsFor: float
      AvgGoalsAgainst: float }

module InsightsQueries =

    let get (db: Database) (playerId: Guid) (selectedTeam: MyTeam.Stats.SelectedTeam) : TeamInsights option =

        let teamIds =
            match selectedTeam with
            | MyTeam.Stats.Team t -> [ t.Id ]
            | MyTeam.Stats.Seven teams -> teams |> List.map (fun t -> t.Id)
            | MyTeam.Stats.Elleven teams -> teams |> List.map (fun t -> t.Id)

        let treningskamp = Nullable <| int Models.Enums.GameType.Treningskamp
        let now = DateTime.Now

        // Get all games where player participated
        let playerGames =
            query {
                for ea in db.EventAttendances do
                    where (
                        teamIds.Contains(ea.Event.TeamId.Value)
                        && ea.Event.GameType <> treningskamp
                        && ea.Event.DateTime < now
                        && ea.MemberId = playerId
                        && ea.IsSelected
                    )

                    select (ea.Event.Id, ea.Event.TeamId.Value, ea.Event.IsHomeTeam, ea.Event.HomeScore, ea.Event.AwayScore)
            }
            |> Seq.toList
            |> List.filter (fun (_, _, _, homeScore, awayScore) -> homeScore.HasValue && awayScore.HasValue)

        if playerGames.IsEmpty then
            None
        else
            // Get game events for this player
            let gameIds =
                playerGames
                |> List.map (fun (gameId, _, _, _, _) -> gameId)

            let gameEvents =
                query {
                    for ge in db.GameEvents do
                        where (gameIds.Contains(ge.GameId))
                        select ge
                }
                |> Seq.toList

            let playerGoals =
                gameEvents
                |> List.filter (fun ge ->
                    ge.Type = GameEventType.Goal
                    && ge.PlayerId = Nullable playerId)
                |> List.length

            let playerAssists =
                gameEvents
                |> List.filter (fun ge ->
                    ge.Type = GameEventType.Goal
                    && ge.AssistedById = Nullable playerId)
                |> List.length

            // Calculate wins
            let wins =
                playerGames
                |> List.filter (fun (_, _, isHomeTeam, homeScore, awayScore) ->
                    if isHomeTeam then
                        homeScore.Value > awayScore.Value
                    else
                        awayScore.Value > homeScore.Value)
                |> List.length

            // Calculate clean sheets
            let cleanSheets =
                playerGames
                |> List.filter (fun (_, _, isHomeTeam, homeScore, awayScore) ->
                    if isHomeTeam then
                        awayScore.Value = 0
                    else
                        homeScore.Value = 0)
                |> List.length

            // Calculate average scorelines
            let goalsFor =
                playerGames
                |> List.map (fun (_, _, isHomeTeam, homeScore, awayScore) ->
                    if isHomeTeam then
                        float homeScore.Value
                    else
                        float awayScore.Value)
                |> List.sum

            let goalsAgainst =
                playerGames
                |> List.map (fun (_, _, isHomeTeam, homeScore, awayScore) ->
                    if isHomeTeam then
                        float awayScore.Value
                    else
                        float homeScore.Value)
                |> List.sum

            let gamesCount = float playerGames.Length

            let teamName =
                match selectedTeam with
                | MyTeam.Stats.Team t -> t.ShortName
                | MyTeam.Stats.Seven _ -> "Samlet 7'er"
                | MyTeam.Stats.Elleven _ -> "Samlet 11'er"

            let teamId =
                match selectedTeam with
                | MyTeam.Stats.Team t -> t.Id
                | MyTeam.Stats.Seven teams -> teams |> List.head |> (fun t -> t.Id)
                | MyTeam.Stats.Elleven teams -> teams |> List.head |> (fun t -> t.Id)

            Some
                { TeamId = teamId
                  TeamName = teamName
                  Games = playerGames.Length
                  Wins = wins
                  WinRate =
                    if gamesCount > 0.0 then
                        (float wins / gamesCount) * 100.0
                    else
                        0.0
                  CleanSheets = cleanSheets
                  GoalsPerGame =
                    if gamesCount > 0.0 then
                        float playerGoals / gamesCount
                    else
                        0.0
                  AssistsPerGame =
                    if gamesCount > 0.0 then
                        float playerAssists / gamesCount
                    else
                        0.0
                  AvgGoalsFor =
                    if gamesCount > 0.0 then
                        goalsFor / gamesCount
                    else
                        0.0
                  AvgGoalsAgainst =
                    if gamesCount > 0.0 then
                        goalsAgainst / gamesCount
                    else
                        0.0 }

