namespace MyTeam

open MyTeam.Games.Events
open MyTeam.Domain
open MyTeam

module GameEventApi =

    
    let getTypes _ =  
        Enums.getValues<GameEventType> ()
        |> Ok
        
       
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
                | None -> Error NotFound
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
                                          GameId = gameId
                                          PlayerId = playerId |> toOption
                                          AssistedById = assistedById |> toOption
                                          Type = enum<GameEventType> (int eventType)
                                    }
                                )
                    |> Ok




