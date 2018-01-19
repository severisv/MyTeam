namespace MyTeam

open MyTeam.Players
open MyTeam.Domain.Members
open Giraffe 

module PlayerApi =

    let list clubId (ctx: HttpContext) =
        Queries.getPlayers ctx.ConnectionString clubId
        |> json, ctx

    let getFacebookIds clubId (ctx: HttpContext) =
        Queries.getFacebookIds ctx.ConnectionString clubId
        |> json, ctx

    
    [<CLIMutable>]
    type SetStatus = {
        Status: Status
    }
    let setStatus id next (ctx: HttpContext) =
          task {
            let! { Status = status } = ctx.BindJsonAsync<SetStatus>()
            printf "-----------------\n %O \n--" status 
            Persistence.setStatus ctx.ConnectionString id status 
            return! next ctx
        }
        