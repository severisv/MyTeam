module MyTeam.Gameevents

open Microsoft.EntityFrameworkCore
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open Shared.Domain.Events
open MyTeam.Validation
open Shared.Validation
open System
open System.Linq
type GameEventId = Guid

    
[<CLIMutable>]
type GameEventForm =
    { Type : string
      PlayerId : PlayerId option
      AssistedById : PlayerId option }

type Get = ClubId -> GameId -> Database -> HttpResult<GameEvent list>

type Add = ClubId -> GameId -> HttpContext -> GameEventForm -> HttpResult<GameEvent>

type Delete = ClubId -> (GameId * GameEventId) -> Database -> HttpResult<unit>

let getTypes _ = Enums.getValues<GameEventType>() |> OkResult

let get : Get =
    fun clubId gameId (db : Database) -> 
        let (ClubId clubId) = clubId
        query { 
            for g in db.Games do
                where (g.Id = gameId && g.Team.ClubId = clubId)
                select (g.Id)
        }
        |> Seq.tryHead
        |> function 
        | None -> NotFound
        | Some gameId -> 
            query { 
                for ge in db.GameEvents do
                    where (ge.GameId = gameId)
                    sortBy (ge.CreatedDate)
                    select (ge.Id, ge.PlayerId, ge.AssistedById, ge.Type)
            }
            |> Seq.toList
            |> List.map (fun (id, playerId, assistedById, eventType) -> 
                   { Id = id
                     PlayerId = playerId |> fromNullable
                     AssistedById = assistedById |> fromNullable
                     Type = match eventType with
                            | Models.Enums.GameEventType.Goal -> Mål 
                            | Models.Enums.GameEventType.YellowCard -> ``Gult kort`` 
                            | _ -> ``Rødt kort`` })
            |> OkResult

let add : Add =
    let cardDoesNotHaveAssist _ (model: GameEvent) =
        if model.Type <> GameEventType.Mål && model.AssistedById.IsSome then 
            Error "Kort kan ikke ha assist"
        else Ok()
    
    let isNotAssistedBySelf _ (model: GameEvent) =
        if model.PlayerId.IsSome && model.PlayerId = model.AssistedById then 
            Error "Man kan ikke gi assist til seg selv"
        else Ok()
    
    fun clubId gameId (ctx : HttpContext) (model: GameEventForm) ->
        let model: GameEvent = { Id = Guid.Empty; Type = Enums.fromString model.Type; PlayerId = model.PlayerId; AssistedById = model.AssistedById }
        let db = ctx.Database
        let (ClubId clubId) = clubId
        query { 
            for g in db.Games do
                where (g.Id = gameId)
                select (g.Team.ClubId)
        }
        |> Seq.tryHead
        |> function 

        | None -> NotFound
        | Some c when c <> clubId -> Unauthorized
        | Some _ ->
            combine  [ <@ model @> >- [ cardDoesNotHaveAssist ]
                       <@ model @> >- [ isNotAssistedBySelf ]
                       <@ model.Type @> >- [ isRequired ] ]
            |> function 
            | Ok() -> 
                let ge = Models.Domain.GameEvent()
                ge.Id <- Guid.NewGuid()
                ge.GameId <- gameId
                ge.PlayerId <- (model.PlayerId |> toNullable)
                ge.AssistedById <- (model.AssistedById |> toNullable)
                ge.Type <- match model.Type with
                            | Mål -> Models.Enums.GameEventType.Goal
                            | ``Gult kort`` -> Models.Enums.GameEventType.YellowCard
                            | ``Rødt kort`` -> Models.Enums.GameEventType.RedCard
                ge.CreatedDate <- DateTime.Now
                db.GameEvents.Add ge  |> ignore
                db.SaveChanges() |> ignore
                { model with Id = ge.Id } 
                |> OkResult
            | Error e -> ValidationErrors e

let delete : Delete =
    fun clubId (gameId, gameEventId) (db : Database) -> 
        let (ClubId clubId) = clubId
        db.GameEvents.Include(fun ge -> ge.Game)
            .Where(fun ge -> ge.Id = gameEventId && ge.GameId = gameId)
        |> Seq.tryHead
        |> function 
        | None -> NotFound
        | Some gameEvent when gameEvent.Game.ClubId <> clubId -> Unauthorized
        | Some gameEvent -> 
            db.GameEvents.Remove gameEvent |> ignore
            db.SaveChanges()
            |> ignore
            |> OkResult
