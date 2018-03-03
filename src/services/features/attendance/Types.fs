namespace MyTeam.Attendance

open MyTeam
open System
open MyTeam.Domain.Members
open MyTeam.Domain

type Training = {
    Id: EventId
    Date: DateTime       
    Location: string
} 
            
type PlayerAttendance = Member * bool

type Players = {
    Attending: PlayerAttendance list
    OthersActive: PlayerAttendance list
    OthersInactive: PlayerAttendance list
}    

type Model = {
    Training: Training
    Players: Players
}

type PlayerAttendanceSummary = {
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
    Attendance: PlayerAttendanceSummary list
    Years: int list
}

type Year = string option
type GetAttendance = Database -> ClubId -> Year -> ShowAttendanceModel
type GetPreviousTrainings = Database -> ClubId -> Training list
type GetTraining = Database -> EventId -> Training
type GetPlayers = Database -> ClubId -> EventId -> Players
type ConfirmAttendance = Database -> ClubId -> EventId -> MemberId -> bool -> Result<unit, Error>