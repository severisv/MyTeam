namespace MyTeam.Attendance

open MyTeam
open System
open MyTeam.Domain.Members
open MyTeam.Domain.Events
open MyTeam.Domain
open MyTeam.Shared.Components.Input

            
            
type PlayerDidAttend = bool            
type PlayerDidWin = bool            
type PlayerAttendance = Member * PlayerDidAttend * PlayerDidWin

type Playerlist = {
    Attending: PlayerAttendance list
    OthersActive: PlayerAttendance list
    OthersInactive: PlayerAttendance list
}    

type AttendanceSummary = {
    Games: int
    Trainings: int
    NoShows: int
    TrainingVictories: int
}   

type PlayerAttendanceSummary = Member * AttendanceSummary

type SelectedYear = 
| AllYears
| Year of int

type Years = int list
type Year = string option



type PeriodStart = DateTime

type GetAttendance = Database -> ClubId -> Year -> SelectedYear * Years * PlayerAttendanceSummary list
type GetPreviousTrainings = Database -> ClubId -> Event list
type GetTraining = Database -> EventId -> Event
type GetPlayers = Database -> ClubId -> EventId -> Playerlist
type ConfirmAttendance = Database -> ClubId -> EventId ->  MemberId -> CheckboxPayload -> HttpResult<unit>
type ConfirmVictory = Database -> ClubId -> EventId ->  MemberId -> CheckboxPayload -> HttpResult<unit>
