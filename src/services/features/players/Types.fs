namespace MyTeam.Players

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

type PlayerId = Guid

type Player = {
    Id:         PlayerId
    FirstName: string
    MiddleName: string
    LastName: string
    UrlName: string
    Status: Status
    Roles: Role list
    TeamIds: TeamId list
} with
    member x.FullName = sprintf "%s %s" x.FirstName x.LastName 

type ListPlayers = ConnectionString -> ClubId -> Player list
