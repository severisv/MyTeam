module MyTeam.Games.Queries

open MyTeam
open Shared
open Shared.Domain
open Shared.Strings
open MyTeam.Common.Features.Members
open System.Linq
open System
open Client.Games.SelectSquad

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
                    select (game.Team.Name, game.Team.Id, game.IsHomeTeam, game.Opponent, game.HomeScore, game.AwayScore, game.DateTime, game.Location, game.GameType, game.GamePlanIsPublished, game.Report.Name, game.Description)
                 }
                 |> Seq.map (fun (name, teamId, isHomeTeam, opponent, homeScore, awayScore, dateTime, location, gameType, gamePlanIsPublished, matchReportName, description) ->
                    {   Id = gameId
                        Team = { Id = teamId; Name = name }
                        IsHomeTeam = isHomeTeam
                        Opponent = !!opponent
                        HomeScore = homeScore |> fromNullable
                        AwayScore = awayScore |> fromNullable
                        DateTime = dateTime
                        Location = !!location
                        Description = !!description
                        Type = Events.gameTypeFromInt gameType.Value
                        GamePlanIsPublished = gamePlanIsPublished |> fromNullable |> Option.defaultValue false
                        MatchReportName = (Strings.hasValue matchReportName) =? (Some matchReportName, None)  })
                 |> Seq.tryHead



let listGameYears: ListGameYears =
    fun db teamId ->
        query {
            for game in db.Games do
            where (game.TeamId = Nullable teamId)
            select game.DateTime.Year
            distinct
        }
        |> Seq.toList
        |> List.sortByDescending id


let listGames: ListGames =
    fun db teamId year ->
                query {
                    for game in db.Games do
                    where (game.TeamId = Nullable teamId && game.DateTime.Year = year)
                    select (game.Id, game.Team.Name, game.IsHomeTeam, game.Opponent, game.HomeScore, game.AwayScore, game.DateTime, game.Location, game.GameType, game.GamePlanIsPublished, game.Report.Name, game.Description)
                 }
                 |> Seq.map (fun (id, name, isHomeTeam, opponent, homeScore, awayScore, dateTime, location, gameType, gamePlanIsPublished, matchReportName, description) ->
                    {
                        Id = id
                        Team = { Id = teamId; Name = name }
                        IsHomeTeam = isHomeTeam
                        Opponent = opponent
                        HomeScore = homeScore |> fromNullable
                        AwayScore = awayScore |> fromNullable
                        DateTime = dateTime
                        Location = location
                        Description = description
                        Type = Events.gameTypeFromInt gameType.Value
                        GamePlanIsPublished = gamePlanIsPublished |> fromNullable |> Option.defaultValue false
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
                    et.Event.Type = (Events.eventTypeToInt EventType.Trening) &&
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
        