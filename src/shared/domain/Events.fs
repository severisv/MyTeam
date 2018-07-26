namespace MyTeam.Shared.Domain


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
        