namespace MyTeam.Attendance

open MyTeam.Domain
open MyTeam.Attendance
open MyTeam
open System
open Queries

module Api =
    
    let confirmAttendance = 
        fun clubId (eventId, playerId) (ctx: HttpContext) model ->
            Persistence.confirmAttendance ctx.Database clubId eventId playerId model

    let getRecentAttendance (club: Club) (teamId: TeamId) db =
            let eightWeeksAgo = DateTime.Now.AddDays(-56.0)
            getRecentAttendance db club teamId eightWeeksAgo
           