namespace MyTeam.Players

open System
open System.Linq
open MyTeam
open MyTeam.Models.Enums
open MyTeam.Stats
open Shared.Domain

type PlayerRelationshipStat =
    { PlayerId: Guid
      PlayerName: string
      Games: int
      Wins: int option
      WinRate: float option
      Assists: int option }

type PlayerRelationships =
    { MostPlayedWith: PlayerRelationshipStat list
      HighestWinrate: PlayerRelationshipStat list
      LowestWinrate: PlayerRelationshipStat list
      MostAssistsTo: PlayerRelationshipStat list
      MostAssistsFrom: PlayerRelationshipStat list
      BestFriends: PlayerRelationshipStat list }

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
      AvgGoalsAgainst: float
      Relationships: PlayerRelationships option }

module InsightsQueries =

    let private getPlayerRelationships
        (db: Database)
        (playerId: Guid)
        (teamIds: Guid list)
        (gameIds: Guid list)
        : PlayerRelationships option =

        let treningskamp = Nullable <| int Models.Enums.GameType.Treningskamp
        let now = DateTime.Now

        if gameIds.IsEmpty then
            None
        else
            // Get all teammates who played in the same games (only IDs and game data)
            let gamesWithTeammates =
                query {
                    for ea in db.EventAttendances do
                        where (
                            teamIds.Contains(ea.Event.TeamId.Value)
                            && ea.Event.GameType <> treningskamp
                            && ea.Event.DateTime < now
                            && gameIds.Contains(ea.EventId)
                            && ea.IsSelected
                            && ea.MemberId <> playerId
                        )

                        select (ea.EventId, ea.MemberId, ea.Event.IsHomeTeam, ea.Event.HomeScore, ea.Event.AwayScore)
                }
                |> Seq.toList
                |> List.filter (fun (_, _, _, homeScore, awayScore) -> homeScore.HasValue && awayScore.HasValue)

            // Group by game to get teammate list per game with win status
            let gameTeammatesMap =
                gamesWithTeammates
                |> List.groupBy (fun (gameId, _, _, _, _) -> gameId)
                |> List.map (fun (gameId, teammates) ->
                    let (_, _, isHomeTeam, homeScore, awayScore) = teammates |> List.head

                    let isWin =
                        if isHomeTeam then
                            homeScore.Value > awayScore.Value
                        else
                            awayScore.Value > homeScore.Value

                    (gameId, isWin))
                |> Map.ofList

            // Calculate statistics for each teammate (work with IDs only)
            let teammateStats =
                gamesWithTeammates
                |> List.groupBy (fun (_, memberId, _, _, _) -> memberId)
                |> List.map (fun (memberId, games) ->
                    let gamesWithTeammate =
                        games
                        |> List.map (fun (gameId, _, _, _, _) -> gameId)
                        |> List.distinct

                    let wins =
                        gamesWithTeammate
                        |> List.filter (fun gameId ->
                            match gameTeammatesMap.TryFind gameId with
                            | Some isWin -> isWin
                            | None -> false)
                        |> List.length

                    let totalGames = gamesWithTeammate.Length

                    let winRate =
                        if totalGames > 0 then
                            (float wins / float totalGames) * 100.0
                        else
                            0.0

                    (memberId, totalGames, wins, winRate))

            // Most played with (top 5 IDs)
            let mostPlayedWithIds =
                teammateStats
                |> List.sortByDescending (fun (_, games, _, _) -> games)
                |> List.take (min 5 teammateStats.Length)
                |> List.map (fun (memberId, games, wins, winRate) -> (memberId, games, Some wins, Some winRate, None))

            // Highest winrate (min 5 games, top 5 IDs)
            let highestWinrateIds =
                teammateStats
                |> List.filter (fun (_, games, _, _) -> games >= 5)
                |> List.sortByDescending (fun (_, _, _, winRate) -> winRate)
                |> List.take (
                    min
                        5
                        (teammateStats
                         |> List.filter (fun (_, games, _, _) -> games >= 5)
                         |> List.length)
                )
                |> List.map (fun (memberId, games, wins, winRate) -> (memberId, games, Some wins, Some winRate, None))

            // Lowest winrate (min 5 games, top 5 IDs)
            let lowestWinrateIds =
                teammateStats
                |> List.filter (fun (_, games, _, _) -> games >= 5)
                |> List.sortBy (fun (_, _, _, winRate) -> winRate)
                |> List.take (
                    min
                        5
                        (teammateStats
                         |> List.filter (fun (_, games, _, _) -> games >= 5)
                         |> List.length)
                )
                |> List.map (fun (memberId, games, wins, winRate) -> (memberId, games, Some wins, Some winRate, None))

            // Get assist relationships (only IDs)
            let goalEventsWithIds =
                query {
                    for ge in db.GameEvents do
                        where (
                            gameIds.Contains(ge.GameId)
                            && ge.Type = GameEventType.Goal
                            && (ge.PlayerId = Nullable playerId
                                || ge.AssistedById = Nullable playerId)
                        )

                        select (ge.PlayerId, ge.AssistedById)
                }
                |> Seq.toList

            // Build assist data with IDs only
            let assistsToData =
                goalEventsWithIds
                |> List.filter (fun (scorerId, assisterId) ->
                    assisterId = Nullable playerId
                    && scorerId.HasValue)
                |> List.groupBy (fun (scorerId, _) -> scorerId.Value)
                |> List.map (fun (scorerId, goals) -> (scorerId, goals |> List.length))

            let assistsFromData =
                goalEventsWithIds
                |> List.filter (fun (scorerId, assisterId) ->
                    scorerId = Nullable playerId
                    && assisterId.HasValue)
                |> List.groupBy (fun (_, assisterId) -> assisterId.Value)
                |> List.map (fun (assisterId, goals) -> (assisterId, goals |> List.length))

            // Most assists to (top 5 IDs)
            let mostAssistsToIds =
                assistsToData
                |> List.sortByDescending snd
                |> List.take (min 5 assistsToData.Length)
                |> List.map (fun (memberId, count) -> (memberId, 0, None, None, Some count))

            // Most assists from (top 5 IDs)
            let mostAssistsFromIds =
                assistsFromData
                |> List.sortByDescending snd
                |> List.take (min 5 assistsFromData.Length)
                |> List.map (fun (memberId, count) -> (memberId, 0, None, None, Some count))

            // Best friends (combined assists to + from, top 5 IDs)
            let assistsToMap = assistsToData |> Map.ofList
            let assistsFromMap = assistsFromData |> Map.ofList

            let allAssistPlayerIds =
                [ assistsToData |> List.map fst
                  assistsFromData |> List.map fst ]
                |> List.concat
                |> List.distinct

            let bestFriendsIds =
                allAssistPlayerIds
                |> List.map (fun playerId ->
                    let assistsTo =
                        assistsToMap
                        |> Map.tryFind playerId
                        |> Option.defaultValue 0

                    let assistsFrom =
                        assistsFromMap
                        |> Map.tryFind playerId
                        |> Option.defaultValue 0

                    let totalAssists = assistsTo + assistsFrom
                    (playerId, totalAssists))
                |> List.sortByDescending snd
                |> List.take (min 5 allAssistPlayerIds.Length)
                |> List.map (fun (memberId, count) -> (memberId, 0, None, None, Some count))

            // Collect all unique player IDs that we need names for
            let allNeededPlayerIds =
                [ mostPlayedWithIds
                  |> List.map (fun (id, _, _, _, _) -> id)
                  highestWinrateIds
                  |> List.map (fun (id, _, _, _, _) -> id)
                  lowestWinrateIds
                  |> List.map (fun (id, _, _, _, _) -> id)
                  mostAssistsToIds
                  |> List.map (fun (id, _, _, _, _) -> id)
                  mostAssistsFromIds
                  |> List.map (fun (id, _, _, _, _) -> id)
                  bestFriendsIds
                  |> List.map (fun (id, _, _, _, _) -> id) ]
                |> List.concat
                |> List.distinct

            // Single query to get all player names we need
            let playerNames =
                if allNeededPlayerIds.IsEmpty then
                    Map.empty
                else
                    query {
                        for m in db.Members do
                            where (allNeededPlayerIds.Contains(m.Id))
                            select (m.Id, m.FirstName, m.LastName)
                    }
                    |> Seq.toList
                    |> List.map (fun (id, firstName, lastName) -> (id, sprintf "%s %s" firstName lastName))
                    |> Map.ofList

            // Helper to convert ID data to PlayerRelationshipStat
            let toStat (memberId, games, wins, winRate, assists) =
                { PlayerId = memberId
                  PlayerName =
                    playerNames
                    |> Map.tryFind memberId
                    |> Option.defaultValue "Unknown"
                  Games = games
                  Wins = wins
                  WinRate = winRate
                  Assists = assists }

            Some
                { MostPlayedWith = mostPlayedWithIds |> List.map toStat
                  HighestWinrate = highestWinrateIds |> List.map toStat
                  LowestWinrate = lowestWinrateIds |> List.map toStat
                  MostAssistsTo = mostAssistsToIds |> List.map toStat
                  MostAssistsFrom = mostAssistsFromIds |> List.map toStat
                  BestFriends = bestFriendsIds |> List.map toStat }

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

            // Get player relationships
            let relationships = getPlayerRelationships db playerId teamIds gameIds

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
                        0.0
                  Relationships = relationships }
