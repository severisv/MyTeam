namespace MyTeam.Members

open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Common.Features
open MyTeam
open Shared.Features.Admin.AddPlayers

module Api =

    let list clubId db =
        Members.list db clubId
        |> OkResult

    [<CLIMutable>]
    type SetStatus = { Status: Status }
    let setStatus clubId id next (ctx: HttpContext) =
            let model = ctx.BindJson<SetStatus>()
            Persistence.setStatus ctx.Database clubId id model.Status 
            |> Tenant.clearUserCache ctx clubId
            next ctx        
    
    [<CLIMutable>]
    type ToggleRole = { Role: Role }
    let toggleRole clubId id next (ctx: HttpContext) =
            let model = ctx.BindJson<ToggleRole>()
            Persistence.toggleRole ctx.Database clubId id model.Role
            |> Tenant.clearUserCache ctx clubId
            next ctx

    [<CLIMutable>]
    type ToggleTeam = { TeamId: TeamId }
    let toggleTeam clubId id (ctx: HttpContext) model =
            Persistence.toggleTeam ctx.Database clubId id model.TeamId
            |> OkResult
           
    let add clubId (ctx: HttpContext) model =
            Persistence.add ctx.Database clubId model            
