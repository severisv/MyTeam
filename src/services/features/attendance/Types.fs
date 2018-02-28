namespace MyTeam.Attendance

open MyTeam
open MyTeam.Models.Enums
open System
open MyTeam.Domain

type EventId = Guid

type Training = {
    Id: EventId
    Date: DateTime       
    Location: string
}

type Player = {
    Id: Guid
    FacebookId: string
    FirstName: string
    LastName: string
    Image: string       
    DidAttend: bool    
    Status: PlayerStatus   
} with 
    member p.Name = sprintf "%s %s" p.FirstName p.LastName        

type Players = {
    Attending: Player list
    OthersActive: Player list
    OthersInactive: Player list
}    

type Model = {
    Training: Training
    Players: Players
}

type PlayerAttendance = {
    FacebookId: string
    FirstName: string
    LastName: string
    UrlName: string
    Image: string
    Games: int
    Trainings: int
    NoShows: int
}   
with member p.Name = sprintf "%s %s" p.FirstName p.LastName    

type SelectedYear = 
| AllYears
| Year of int

type ShowAttendanceModel = {
    SelectedYear: SelectedYear
    Attendance: PlayerAttendance list
    Years: int list
}

type GetAttendance = Database -> ClubId -> string option -> ShowAttendanceModel
type GetPreviousTrainings = Database -> ClubId -> Training list
type GetTraining = Database -> EventId -> Training
type GetPlayers = Database -> ClubId -> EventId -> Players
type ConfirmAttendance = Database -> ClubId -> EventId -> MemberId -> bool -> Result<unit, Error>