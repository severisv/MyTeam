namespace MyTeam.Attendance

open MyTeam.Attendance
open MyTeam


module Api =
    
    let confirmAttendance = 
        fun clubId (eventId, playerId) (ctx: HttpContext) model ->
            Persistence.confirmAttendance ctx.Database clubId eventId playerId model

           