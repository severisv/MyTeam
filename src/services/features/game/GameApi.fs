module MyTeam.Games.Api

open Server
open Shared.Domain
open MyTeam
open Shared
open System
open System.Linq
open Giraffe
open Client.Features.Games.Add


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


[<CLIMutable>]
type PostScore = { value: Nullable<int> }

let setHomeScore clubId gameId (ctx: HttpContext) model =
    updateGame clubId gameId ctx.Database (fun game -> game.HomeScore <- model.value)

let setAwayScore clubId gameId (ctx: HttpContext) model =
    updateGame clubId gameId ctx.Database (fun game -> game.AwayScore <- model.value)


[<CLIMutable>]
type GamePlanModel = { GamePlan: string }

let setGamePlan clubId gameId next (ctx: HttpContext) =
    let gamePlan =
        ctx.ReadBodyFromRequestAsync().ConfigureAwait(false).GetAwaiter().GetResult()

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
            Notifications.clearCache ctx clubId
            r)


let getInsights = Insights.get

let add (club: Club) (ctx: HttpContext) (model: AddGame) =
    let db = ctx.Database
    let (ClubId clubId) = club.Id

    let game =
        Models.Domain.Event(
              ClubId = clubId,                
              DateTime = model.Date + model.Time, 
              Opponent = model.Opponent,            
              Location = model.Location,
              TeamId = Nullable model.Team, 
              GameType = Nullable (Events.gameTypeToInt model.GameType),
              Type = Events.eventTypeToInt Kamp,
              Description = model.Description,
              IsHomeTeam = model.IsHomeGame)   

    db.Events.Add(game) |> ignore
    db.SaveChanges() |> ignore
    OkResult { model with Id = Some game.Id; Date = game.DateTime }
