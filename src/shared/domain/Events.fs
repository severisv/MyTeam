namespace Shared.Domain
open System

type EventType = 
        | Alle = 0
        | Trening = 1
        | Kamp = 2
        | Diverse = 3
    

type GameType =
    | Treningskamp = 0
    | Seriekamp = 1
    | Norgesmesterskapet = 2
    | Kretsmesterskapet = 3
    | ``OBOS Cup`` = 4


module Events = 
        type EventId = Guid
        type GameId = EventId

        type Event = {
            Id: EventId
            Date: DateTime       
            Location: string
        } 
