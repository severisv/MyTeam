namespace MyTeam.Attendance

open MyTeam
open System
open MyTeam.Domain.Members
open MyTeam.Domain.Events
open MyTeam.Domain
            
type PlayerDidAttend = bool            
type PlayerAttendance = Member * PlayerDidAttend

type Playerlist = {
    Attending: PlayerAttendance list
    OthersActive: PlayerAttendance list
    OthersInactive: PlayerAttendance list
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

type Years = int list
type Year = string option

type GetAttendance = Database -> ClubId -> Year -> SelectedYear * Years * PlayerAttendanceSummary list
type GetPreviousTrainings = Database -> ClubId -> Training list
type GetTraining = Database -> EventId -> Training
type GetPlayers = Database -> ClubId -> EventId -> Playerlist
type ConfirmAttendance = Database -> ClubId -> EventId -> MemberId -> bool -> Result<unit, Error>