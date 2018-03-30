namespace MyTeam.Attendance

open MyTeam
open System
open MyTeam.Domain.Members
open MyTeam.Domain.Events
open MyTeam.Domain
open MyTeam.Ajax
            
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

type Years = int list
type Year = string option

type TeamAttendance = {
    MemberId: MemberId
    AttendancePercentage: int
}

type PeriodStart = DateTime

type GetAttendance = Database -> ClubId -> Year -> SelectedYear * Years * PlayerAttendanceSummary list
type GetPreviousTrainings = Database -> ClubId -> Event list
type GetTraining = Database -> EventId -> Event
type GetPlayers = Database -> ClubId -> EventId -> Playerlist
type ConfirmAttendance = ClubId -> EventId * MemberId -> Database -> CheckboxPayload -> Result<unit, Error>
type GetRecentAttendance = Database -> Club -> TeamId -> PeriodStart -> Result<TeamAttendance list, Error>

