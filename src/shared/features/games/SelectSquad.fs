module Shared.Features.Games.SelectSquad

open System
open Shared.Domain.Members
open Shared.Image

type Signup = {
        MemberId: Guid
        IsAttending: bool
        Message: string        
    }
    
type TeamAttendance = {
    MemberId: MemberId
    AttendancePercentage: int
}

type Player = MemberWithTeamsAndRoles * Signup option

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
    TeamId: Guid
}


type Model = {
    Game: GameDetailed
    ImageOptions: CloudinaryOptions
    Signups: Signup list
    Members: MemberWithTeamsAndRoles list
    RecentAttendance: TeamAttendance list
}

let clientView = "select-squad"
let modelAttribute = "model"