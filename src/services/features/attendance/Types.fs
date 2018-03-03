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
            
type PlayerDidAttend = bool            
type PlayerAttendance = Member * PlayerDidAttend

type Players = {
    Attending: PlayerAttendance list
    OthersActive: PlayerAttendance list
    OthersInactive: PlayerAttendance list
}    

type Model = {
    Training: Training
    Players: Players
}

type AttendanceSummary = {
    Games: int
    Trainings: int
    NoShows: int
}   

type PlayerAttendanceSummary = Member * AttendanceSummary

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