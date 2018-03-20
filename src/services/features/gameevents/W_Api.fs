namespace MyTeam

open MyTeam.Games.Events
open MyTeam.Domain
open MyTeam
open Giraffe 
open System

module GameEventApi =

    
    let getTypes _ =   
        Enums.getValues<GameEventType> ()
        |> json
        
       
    let get : Get =
        fun clubId gameId (db: Database) ->
            let (ClubId clubId) = clubId
            db.Games 
            |> Seq.tryFind (fun g -> g.Id = gameId && g.Team.ClubId = clubId)
            |> function
                | None -> Error NotFound
                | Some game ->
                    query {
                        for ge in db.GameEvents do
                        where (ge.GameId = game.Id && ge.Game.ClubId = clubId)
                        sortBy (ge.CreatedDate)
                        select ({
                                  Id = ge.Id
                                  GameId = game.Id
                                  PlayerId = ge.PlayerId |> toOption
                                  AssistedById = ge.AssistedById |> toOption
                                  Type = enum<GameEventType> (int ge.Type)
                        })
                    }
                    |> Seq.toList
                    |> Ok


