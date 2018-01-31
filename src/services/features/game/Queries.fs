namespace MyTeam.Games

open MyTeam
open MyTeam.Domain
open Microsoft.EntityFrameworkCore
open System.Linq

module Queries =

    let games (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Games.Where(fun p -> p.ClubId = clubId)


