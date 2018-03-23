namespace MyTeam

open MyTeam.Domain
open MyTeam.Attendance
open System
open Queries

module AttendanceApi =
    
    let confirmAttendance = Persistence.confirmAttendance

    let getRecentAttendance (club: Club) (teamId: TeamId) db =
            let eightWeeksAgo = DateTime.Now.AddDays(-56.0)
            getRecentAttendance db club teamId eightWeeksAgo
           