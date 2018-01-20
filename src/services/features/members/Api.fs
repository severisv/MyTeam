namespace MyTeam

open MyTeam.Members
open MyTeam.Domain
open MyTeam.Domain.Members
open Giraffe 

module MemberApi =

    
    let list clubId (ctx: HttpContext) =
        Queries.list ctx.ConnectionString clubId
        |> json, ctx


    let getFacebookIds clubId (ctx: HttpContext) =
        Queries.getFacebookIds ctx.ConnectionString clubId
        |> json, ctx


    [<CLIMutable>]
    type SetStatus = {
        Status: Status
    }
    let setStatus club id next (ctx: HttpContext) =
            let model = ctx.BindJson<SetStatus>()
            Persistence.setStatus ctx.ConnectionString club.Id id model.Status 
            next ctx
        
        