namespace MyTeam.Events

open MyTeam
open MyTeam.Events
open MyTeam.Shared.Components.Input

module Api =
    let setDescription clubId eventId next (ctx : HttpContext) =
        let model = ctx.BindJson<StringPayload>()
        Persistence.setDescription ctx.Database clubId eventId model.Value
        next ctx
