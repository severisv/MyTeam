namespace MyTeam

open MyTeam
open MyTeam.Domain
open MyTeam.Attendance
open Persistence

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