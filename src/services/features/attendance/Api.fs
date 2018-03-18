namespace MyTeam

open MyTeam
open MyTeam.Domain
open MyTeam.Attendance
open Persistence
open System
open Queries


module AttendanceApi =
    
    [<CLIMutable>]
    type CheckboxPayload = {
        value: bool
    }    
   
    let confirmAttendance (club: Club) eventId playerId next (ctx: HttpContext) =           

            let db = ctx.Database                
            let payload = ctx.BindJson<CheckboxPayload>()

            confirmAttendance db club.Id eventId playerId payload.value
            |> fromResult next ctx


    
    let getRecentAttendance (club: Club) (teamId: TeamId) next (ctx: HttpContext) =

            let eightWeeksAgo = DateTime.Now.AddDays(-56.0)
            getRecentAttendance ctx.Database club teamId eightWeeksAgo
            |> fromResult next ctx
           