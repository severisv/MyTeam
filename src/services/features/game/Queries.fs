namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open System.Linq

module Queries =

    let games (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Games.Where(fun p -> p.ClubId = clubId)


    let getSquad : GetSquad = 
        fun db gameId ->
            query {
                for e in db.EventAttendances do
                where (e.EventId = gameId && e.IsSelected)
                select e.Member
            }
            |> selectMembers
            |> Seq.toList            
