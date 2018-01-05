namespace MyTeam.Services

open Giraffe
open Microsoft.AspNetCore.Builder
open Services

module App =
    
    let webApp =
        choose [
            route "/api/giraffe/getplayers"   >=> json Players.Queries.getPlayers
               ]


    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp

