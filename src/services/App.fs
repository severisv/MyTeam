namespace MyTeam.Services

open Giraffe
open Microsoft.AspNetCore.Builder
open Services

module App =

    let webApp =
        choose [
            GET >=> routeCif "/api/players/%s" (fun id -> System.Guid(id) |> Players.Api.players)
               ]

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp

