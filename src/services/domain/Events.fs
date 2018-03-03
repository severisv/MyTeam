module MyTeam.Domain.Events

open System

type EventId = Guid

type Training = {
    Id: EventId
    Date: DateTime       
    Location: string
} 