module Server.Features.Events.List

open System
open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Client.Events
open Client.Events.List


let internal view (period: Period) (club: Club) (user: User) (ctx: HttpContext) =

    let db = ctx.Database
    let inTwoHours = DateTime.Now.AddHours -2.0
    let (ClubId clubId) = club.Id

    let years =
        match period with
        | Previous _ ->
            query {
                for event in db.Events do
                    where (
                        event.ClubId = clubId
                        && event.DateTime < inTwoHours
                    )

                    select event.DateTime.Year
                    distinct
            }
            |> Seq.toList
            |> List.sortDescending
        | Upcoming _ -> []

    Events.Api.listEvents club user period ctx.Database
    |> HttpResult.map (fun events ->
        [ Client.isomorphicViewOld
              containerId
              element
              { Events = events
                Years = years
                User = user
                Period = period } ]
        |> layout club (Some user) (fun o -> { o with Title = "Påmelding" }) ctx)

let upcoming (club: Club) (user: User) (ctx: HttpContext) =
    view (Upcoming NearFuture) club user ctx

let previous (club: Club) (user: User) year (ctx: HttpContext) = view (Previous year) club user ctx
