namespace Services.Players

open System

type Status =
        | Aktiv = 0
        | Inaktiv = 1
        | Veteran = 2
        | Trener = 3
        | Sluttet = 4 

type Player = {
    Id:         Guid
    FirstName: string
    MiddleName: string
    LastName: string
    UrlName: string
    Status: Status
    Roles: string list
} with
    member x.FullName = sprintf "%s %s" x.FirstName x.LastName 