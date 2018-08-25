module MyTeam.Games.Queries

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System.Linq
open MyTeam.Shared.Domain

let games (db : Database) clubId = 
    let (ClubId clubId) = clubId
    db.Games.Where(fun p -> p.ClubId = clubId)


let getSquad : GetSquad = 
    fun db gameId ->
        query {
            for e in db.EventAttendances do
            where (e.EventId = gameId && e.IsSelected)
            select e.Member
        }
        |> selectMembers
        |> Seq.toList            


let getGame: GetGame =
    fun db gameId ->
                query {
                    for game in db.Games do
                    where (game.Id = gameId)
                    select (game.Team.Name, game.IsHomeTeam, game.Opponent, game.HomeScore, game.AwayScore, game.DateTime, game.Location, game.GameType, game.GamePlanIsPublished, game.Report.Name)
                 }
                 |> Seq.map (fun (name, isHomeTeam, opponent, homeScore, awayScore, dateTime, location, gameType, gamePlanIsPublished, matchReportName) ->
                    {
                        Id = gameId
                        IsHomeTeam = isHomeTeam
                        HomeTeam = isHomeTeam =? (name, opponent)
                        AwayTeam = isHomeTeam =? (opponent, name)
                        HomeScore = homeScore |> toOption
                        AwayScore = awayScore |> toOption
                        DateTime = dateTime
                        Location = location
                        Type = enum<GameType> (gameType.Value)
                        GamePlanIsPublished = gamePlanIsPublished |> toOption |> Option.defaultValue false
                        MatchReportName = (hasValue matchReportName) =? (Some matchReportName, None)
                    }                    
                 )
                 |> Seq.tryHead



let listGameYears: ListGameYears =
    fun db teamId ->
        query {
            for game in db.Games do
            where (game.TeamId = teamId)
            select game.DateTime.Year
            distinct
        }
        |> Seq.toList
        |> List.sortByDescending id


let listGames: ListGames =
    fun db teamId year ->
                query {
                    for game in db.Games do
                    where (game.TeamId = teamId && game.DateTime.Year = year)
                    select (game.Id, game.Team.Name, game.IsHomeTeam, game.Opponent, game.HomeScore, game.AwayScore, game.DateTime, game.Location, game.GameType, game.GamePlanIsPublished, game.Report.Name)
                 }
                 |> Seq.map (fun (id, name, isHomeTeam, opponent, homeScore, awayScore, dateTime, location, gameType, gamePlanIsPublished, matchReportName) ->
                    {
                        Id = id
                        IsHomeTeam = isHomeTeam
                        HomeTeam = isHomeTeam =? (name, opponent)
                        AwayTeam = isHomeTeam =? (opponent, name)
                        HomeScore = homeScore |> toOption
                        AwayScore = awayScore |> toOption
                        DateTime = dateTime
                        Location = location
                        Type = enum<GameType> (gameType.Value)
                        GamePlanIsPublished = gamePlanIsPublished |> toOption |> Option.defaultValue false
                        MatchReportName = (hasValue matchReportName) =? (Some matchReportName, None)
                    }                    
                 )
                 |> Seq.toList
                 |> List.sortBy(fun g -> g.DateTime)
