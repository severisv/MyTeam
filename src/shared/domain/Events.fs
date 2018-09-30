namespace MyTeam.Domain
open System

type EventType = 
        | Alle
        | Trening
        | Kamp
        | Diverse
    

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
