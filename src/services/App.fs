namespace MyTeam.Services

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Services

module App =
     
    let webApp =
        fun next ctx ->
            let connectionString = getConnectionString ctx

            choose [
                GET >=> routeCif "/api/players/%s" (parseGuid >> PlayerApi.list connectionString)                                    
                                                    
                   ] next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


