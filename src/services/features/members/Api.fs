namespace MyTeam.Members

open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam
open Giraffe 

module Api =

    let list clubId db =
        Queries.list db clubId
        |> OkResult


    let getFacebookIds clubId db =
        Queries.getFacebookIds db clubId
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
    let toggleTeam clubId id db model =
            Persistence.toggleTeam db clubId id model.TeamId
            |> OkResult
           
    let add clubId db model =
            Persistence.add db clubId model            
