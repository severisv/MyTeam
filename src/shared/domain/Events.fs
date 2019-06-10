namespace Shared.Domain
open System

type EventType = 
    | Alle
    | Trening
    | Kamp
    | Diverse
    


type GameType =
    | Treningskamp
    | Seriekamp
    | Norgesmesterskapet
    | Kretsmesterskapet
    | ``OBOS Cup``


module Events =
        
    let eventTypeFromInt =
        function
         | 0 -> Alle
         | 1 -> Trening
         | 2 -> Kamp
         | _ -> Diverse

    let eventTypeToInt =
        function
         | Alle -> 0
         | Trening -> 1
         | Kamp -> 2
         | Diverse -> 3
         
    let gameTypeFromInt =
        function
         | 0 -> Treningskamp
         | 1 -> Seriekamp
         | 2 -> Norgesmesterskapet
         | 3 -> Kretsmesterskapet
         | _ -> ``OBOS Cup``

    let gameTypeToInt =
        function
         | Treningskamp -> 0
         | Seriekamp -> 1
         | Norgesmesterskapet -> 2
         | Kretsmesterskapet -> 3
         | ``OBOS Cup`` -> 4
        
        
    type EventId = Guid
    type GameId = EventId

    type Event = {
        Id: EventId
        Date: DateTime       
        Location: string
    } 
