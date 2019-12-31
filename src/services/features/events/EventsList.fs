module Server.Features.Events.List

open Shared.Domain
open Shared.Domain.Members
open MyTeam
open Client.Events
open Client.Events.List


let internal view (period: Period) (club : Club) (user : User) (ctx : HttpContext) =
     
     Events.Api.listEvents club user period ctx.Database        
     |> HttpResult.map(fun events ->
         [ Client.view
                containerId
                element
                { Events = events
                  User = user
                  Period = period } ]
         |> layout club (Some user) (fun o -> { o with Title = "Påmelding" }) ctx
     )
let upcoming (club : Club) (user : User) (ctx : HttpContext) = view (Upcoming NearFuture) club user ctx
let previous (club : Club) (user : User) (ctx : HttpContext) = view (Previous None) club user ctx
