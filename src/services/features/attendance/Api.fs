namespace MyTeam

open MyTeam
open MyTeam.Domain
open MyTeam.Attendance
open System
open Queries
open Request

module AttendanceApi =
    
    let confirmAttendance = Persistence.confirmAttendance

    let getRecentAttendance (club: Club) (teamId: TeamId) next (ctx: HttpContext) =

            let eightWeeksAgo = DateTime.Now.AddDays(-56.0)
            getRecentAttendance ctx.Database club teamId eightWeeksAgo
            |> fromResult next ctx
           