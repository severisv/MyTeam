namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System.Linq
open Microsoft.EntityFrameworkCore

module Queries =

    let games (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Games.Where(fun p -> p.ClubId = clubId)


    let getSquad : GetSquad = 
        fun db clubId gameId ->
            db.EventAttendances.Include(fun e -> e.Member)
            |> Seq.filter(fun e -> e.EventId = gameId && e.IsSelected)
            |> Seq.map(fun g ->
                        {                      
                            Id = g.Member.Id
                            FirstName = g.Member.FirstName
                            MiddleName = g.Member.MiddleName
                            LastName = g.Member.LastName
                            UrlName = g.Member.UrlName                                        
                        }
                )
            |> Seq.toList            
            |> List.sortBy(fun m -> m.FirstName)