namespace MyTeam.Games

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Models
open MyTeam.Models.Domain
open MyTeam.Validation

module Persistence =
           

                                                
    let (=>) result fn =
        result |> Result.bind(fn)    

    let setScore : SetScore =
        fun db clubId gameId form ->  
                     
           let game = Queries.games db clubId 
                      |> Seq.find(fun game -> game.Id = gameId)                    

           form.Home |> Option.map(fun s -> game.HomeScore <- Nullable(s)) |> ignore
           form.Away |> Option.map(fun s -> game.AwayScore <- Nullable(s)) |> ignore
           db.SaveChanges() |> ignore
           Ok ()
                    

