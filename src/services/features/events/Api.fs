namespace MyTeam

open MyTeam.Events
open MyTeam.Domain
open Giraffe 

module EventApi =

    
    [<CLIMutable>]
    type Description = {
        Description: string
    }
    let setDescription clubId eventId next (ctx: HttpContext) =
            let model = ctx.BindJson<Description>()
            Persistence.setDescription ctx.Database clubId eventId model.Description
            next ctx            