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
        
       