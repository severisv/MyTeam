module Server.Features.Fines.RemedyRates

open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Client.Fines.RemedyRates

let view (club: Club) (user: User) (ctx: HttpContext) =

    let db = ctx.Database
    let rates = Api.listRemedyRates club db

    [ Client.isomorphicViewOld
          containerId
          element
          { Rates = rates
            Path = ctx.Request.Path.Value
            User = user } ]
    |> layout club (Some user) (fun o -> { o with Title = "Bøtesatser" }) ctx
    |> OkResult
