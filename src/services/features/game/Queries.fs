module MyTeam.Games.Queries

open MyTeam
open MyTeam.Domain
open MyTeam.Common.Features.Members
open System.Linq
open System
open Shared.Features.Games.SelectSquad


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
    fun db clubId gameId ->
                let (ClubId clubId) = clubId
                query {
                    for game in db.Games do
                    where (game.Id = gameId && game.ClubId = clubId)
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
                        MatchReportName = (Strings.hasValue matchReportName) =? (Some matchReportName, None)
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
                        MatchReportName = (Strings.hasValue matchReportName) =? (Some matchReportName, None)
                    }                    
                 )
                 |> Seq.toList
                 |> List.distinctBy (fun g -> g.Id) // Fordi det er en feil med databasemodellen som gjør at mange artikler kan peke på samme kamp
                 |> List.sortBy(fun g -> g.DateTime)



let getRecentAttendance : GetRecentAttendance = 
    fun db teamId ->

        let periodStart = DateTime.Now.AddDays(-56.0)
        let now = DateTime.Now

        let eventIds = 
            query {
                for et in db.EventTeams do
                where (
                    et.TeamId = teamId &&
                    et.Event.Type = (int EventType.Trening) &&
                    et.Event.DateTime <= now &&
                    et.Event.DateTime >= periodStart &&
                    et.Event.Voluntary = false
                )                 
                select(et.EventId)
            } |> Seq.toList

        let eventAttendences = 
            query {
                for ea in db.EventAttendances do
                where (eventIds.Contains(ea.EventId) && ea.DidAttend)
                select (ea.EventId, ea.MemberId)
            } |> Seq.toList

        let eventCount = eventIds |> Seq.length

        eventAttendences 
        |> Seq.groupBy (fun (_, memberId) -> memberId) 
        |> Seq.map (fun (memberId, values) -> 
            {
                MemberId = memberId
                AttendancePercentage = (Seq.length values) * 100 / eventCount
            }
        )
        |> Seq.toList
        