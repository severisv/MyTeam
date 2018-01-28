namespace MyTeam

open MyTeam.Members
open MyTeam.Domain
open MyTeam.Domain.Members
open Giraffe 

module MemberApi =

    let list clubId (ctx: HttpContext) =
        Queries.list ctx.Database clubId
        |> json


    let getFacebookIds clubId (ctx: HttpContext) =
        Queries.getFacebookIds ctx.Database clubId
        |> json


    [<CLIMutable>]
    type SetStatus = {
        Status: Status
    }
    let setStatus clubId id next (ctx: HttpContext) =
            let model = ctx.BindJson<SetStatus>()
            Persistence.setStatus ctx.Database clubId id model.Status 
            |> Tenant.clearUserCache ctx clubId
            next ctx
        
    
    [<CLIMutable>]
    type ToggleRole = {
        Role: Role
    }
    let toggleRole clubId id next (ctx: HttpContext) =
            let model = ctx.BindJson<ToggleRole>()
            Persistence.toggleRole ctx.Database clubId id model.Role
            |> Tenant.clearUserCache ctx clubId
            next ctx    

    [<CLIMutable>]
    type ToggleTeam = {
        TeamId: TeamId
    }
    let toggleTeam clubId id next (ctx: HttpContext) =
            let model = ctx.BindJson<ToggleTeam>()
            Persistence.toggleTeam ctx.Database clubId id model.TeamId
            next ctx            



    let add clubId (ctx: HttpContext) =
            ctx.BindJson<AddMemberForm>() 
            |> Persistence.add ctx.Database clubId
            |> json                