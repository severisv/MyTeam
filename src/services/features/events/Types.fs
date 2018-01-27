namespace MyTeam.Events

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

type EventId = Guid

type Event = {
    Id: EventId
    Description: string
}

type SetDescription = Database -> ClubId -> EventId -> string -> unit
