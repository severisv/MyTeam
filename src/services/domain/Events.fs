module MyTeam.Domain.Events

open System
open System.Linq
open MyTeam

type EventId = Guid

type Training = {
    Id: EventId
    Date: DateTime       
    Location: string
} 


let selectEvents (events: IQueryable<Models.Domain.Event>) = 
        query {
            for e in events do
                        select (e.Id, e.Location, e.DateTime)
        } |> Seq.map(fun (id, location, date) ->
                        {
                            Id = id
                            Date = date
                            Location = location                    
                        }
                    )