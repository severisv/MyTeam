module Shared.Features.Games.SelectSquad

open System
open MyTeam
open MyTeam.Domain.Members

type Signup = {
        MemberId: Guid
        IsAttending: bool
        Message: string        
    }
    

type RecentAttendance = {
    MemberId: Guid
    AttendancePercentage: int
}

type Player = Member * Signup option

type Squad = {
    MemberIds: Guid list
    IsPublished: bool
}

type GameDetailed = {
    Id: Guid
    Date: DateTime
    Location: string
    Description: string
    Squad: Squad
}

type Model = {
    Game: GameDetailed
    ImageOptions: CloudinarySettings
    Signups: Signup list
}

let clientView = "select-squad"
let modelAttribute = "model"