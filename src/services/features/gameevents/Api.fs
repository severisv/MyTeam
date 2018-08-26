namespace MyTeam.Games.Events

open MyTeam.Domain
open MyTeam
open Microsoft.EntityFrameworkCore
open MyTeam.Validation
open System


module Api =
    
    let getTypes _ =  
        Enums.getValues<GameEventType> ()
        |> OkResult
        
       
    let get : Get =
        fun clubId gameId (db: Database) ->
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
                        select (ge.Id, gameId, ge.PlayerId, ge.AssistedById, ge.Type)
                    }
                    |> Seq.toList
                    |> List.map(fun (id, gameId, playerId, assistedById, eventType) ->
                                        {
                                          Id = id
                                          PlayerId = playerId |> toOption
                                          AssistedById = assistedById |> toOption
                                          Type = enum<GameEventType> (int eventType)
                                        }
                                )
                    |> OkResult


    let add : Add =
        let cardDoesNotHaveAssist (_, model) =
            if model.Type <> GameEventType.``Mål`` && model.AssistedById.IsSome 
                then Error "Kort kan ikke ha assist"
            else 
                Ok ()

        let isNotAssistedBySelf (_, model) =
            if model.PlayerId.IsSome && model.PlayerId = model.AssistedById 
                then Error "Man kan ikke gi assist til seg selv"
            else 
                Ok ()

        fun clubId gameId (ctx:HttpContext) model ->
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
                    
                    model ==>
                    [
                       <@ model @> >- [cardDoesNotHaveAssist]
                       <@ model @> >- [isNotAssistedBySelf]
                       <@ model.Type @> >- [isRequired]
                    ] 
                    >>= fun model ->  

                        let ge = Models.Domain.GameEvent()
                        ge.Id <- Guid.NewGuid()
                        ge.GameId <- gameId
                        ge.PlayerId <- (model.PlayerId |> toNullable)
                        ge.AssistedById <- (model.AssistedById |> toNullable)
                        ge.Type <- enum<Models.Enums.GameEventType>(int model.Type)
                        ge.CreatedDate <- DateTime.Now

                        db.GameEvents.Add(ge) |> ignore
                        db.SaveChanges() |> ignore
                        { model with Id = ge.Id } 
                        |> OkResult




    let delete : Delete =
        fun clubId (gameId, gameEventId) (db: Database) ->
            let (ClubId clubId) = clubId
            db.GameEvents.Include(fun ge -> ge.Game) 
            |> Seq.tryFind(fun ge -> ge.Id = gameEventId && ge.GameId = gameId)
            |> function
                | None -> NotFound
                | Some gameEvent when gameEvent.Game.ClubId <> clubId -> Unauthorized
                | Some gameEvent ->
                    db.GameEvents.Remove(gameEvent) |> ignore
                    db.SaveChanges() 
                    |> ignore
                    |> OkResult

