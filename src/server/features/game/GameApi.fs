module MyTeam.Games.Api

open Server
open Shared.Domain
open MyTeam
open Shared
open System
open System.Linq
open Giraffe
open Client.Features.Games.Form

let internal updateGame clubId gameId (db: Database) updateGame =
    db.Games.Where(fun g -> g.Id = gameId)
    |> Seq.tryHead
    |> function
        | Some game when (ClubId game.ClubId) <> clubId -> Unauthorized
        | Some game ->
            updateGame game
            db.SaveChanges() |> OkResult
        | None -> NotFound


let getSquad gameId db = Queries.getSquad db gameId |> OkResult


let setHomeScore clubId gameId (ctx: HttpContext) (model: Components.Input.StringPayload) =
    let value =
        match model.Value with
        | "" -> None
        | v -> Number.tryParse v

    updateGame clubId gameId ctx.Database (fun game -> game.HomeScore <- value |> toNullable)

let setAwayScore clubId gameId (ctx: HttpContext) (model: Components.Input.StringPayload) =
    let value =
        match model.Value with
        | "" -> None
        | v -> Number.tryParse v

    updateGame clubId gameId ctx.Database (fun game -> game.AwayScore <- value |> toNullable)


[<CLIMutable>]
type GamePlanModel = { GamePlan: string }

let setGamePlan clubId gameId next (ctx: HttpContext) =
    let gamePlan =
        ctx
            .ReadBodyFromRequestAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()

    updateGame clubId gameId ctx.Database (fun game -> game.GamePlanState <- gamePlan)
    |> Results.jsonResult next ctx

let publishGamePlan clubId gameId (ctx: HttpContext) _ =
    updateGame clubId gameId ctx.Database (fun game -> game.GamePlanIsPublished <- Nullable true)

let selectPlayer clubId (gameId, playerId) (ctx: HttpContext) model =
    Persistence.selectPlayer ctx.Database clubId gameId playerId model

let publishSquad =
    fun clubId gameId (ctx: HttpContext) _ ->
        Persistence.publishSquad ctx.Database clubId gameId
        |> Results.map (fun r ->
            Notifications.clearCacheForClub ctx clubId
            r)


let getInsights = Insights.get

let add (club: Club) (ctx: HttpContext) (model: AddGame) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let game =
        Models.Domain.Event(
            ClubId = clubId,
            DateTime = model.Date.ToLocalTime().Date + model.Time,
            Opponent = model.Opponent,
            Location = model.Location,
            TeamId = Nullable model.Team,
            GameType = Nullable(Events.gameTypeToInt model.GameType),
            Type = Events.eventTypeToInt Kamp,
            Description = model.Description,
            IsHomeTeam = model.IsHomeGame
        )

    db.Events.Add(game) |> ignore
    db.SaveChanges() |> ignore
    Notifications.clearCacheForClub ctx club.Id

    OkResult
        { model with
            Id = Some game.Id
            Date = game.DateTime }

let update (club: Club) gameId (ctx: HttpContext) (model: AddGame) =
    updateGame club.Id gameId ctx.Database (fun game ->
        game.Opponent <- model.Opponent
        game.DateTime <- model.Date.ToLocalTime().Date + model.Time
        game.Location <- model.Location
        game.TeamId <- Nullable model.Team
        game.GameType <- Nullable(Events.gameTypeToInt model.GameType)
        game.Description <- model.Description
        game.IsHomeTeam <- model.IsHomeGame
        Notifications.clearCacheForClub ctx club.Id)
    |> HttpResult.map (fun _ -> model)


let delete (club: Club) gameId ctx =
    Notifications.clearCacheForClub ctx club.Id
    let db = ctx.Database
    updateGame club.Id gameId db (db.Remove >> ignore)
