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
    Roles: string list
    TeamIds: TeamId list
} with
    member x.FullName = sprintf "%s %s" x.FirstName x.LastName 

type GetPlayers = ConnectionString -> ClubId -> Player list
type GetFacebookIds = ConnectionString -> ClubId -> string list
type SetStatus = ConnectionString -> PlayerId -> Status -> unit
