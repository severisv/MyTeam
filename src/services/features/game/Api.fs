namespace MyTeam.Games

open MyTeam.Domain
open MyTeam
open System

module internal Helpers =
    let updateGame clubId gameId db updateGame  =
        Queries.games db clubId
          |> Seq.tryFind(fun g -> g.Id = gameId)
          |> function
           | Some game when (ClubId game.ClubId) <> clubId -> Error AuthorizationError
           | Some game -> 
                updateGame game
                db.SaveChanges()
                |> Ok
           | None -> Error NotFound      

open Helpers

module Api =

    let getSquad clubId gameId db =
        Queries.getSquad db clubId gameId
        |> Ok
 
    
    [<CLIMutable>]
    type PostScore = { value: Nullable<int> }
    let setHomeScore clubId gameId db model  =
            updateGame clubId gameId db (fun game -> game.HomeScore <- model.value)

    let setAwayScore clubId gameId db model  =
           updateGame clubId gameId db (fun game -> game.AwayScore <- model.value)


    [<CLIMutable>]
    type GamePlanModel = { GamePlan: string }
    let setGamePlan clubId gameId db model  =
        updateGame clubId gameId db (fun game -> game.GamePlan <- model.GamePlan)
 
    let publishGamePlan clubId gameId db _  =
        updateGame clubId gameId db (fun game -> game.GamePlanIsPublished <- Nullable true)

    let selectPlayer = Persistence.selectPlayer        