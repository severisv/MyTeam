module MyTeam.Domain.Eventqueries

open System
open System.Linq
open MyTeam
open MyTeam.Domain.Events
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