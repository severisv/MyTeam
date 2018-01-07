namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open MyTeam

module App =

    let getNils =
        fun (ctx: HttpContext) ->
            json ["Nils"], ctx

    let webApp =
        fun next ctx ->
            let connectionString = getConnectionString ctx
            let clubId = System.Guid("6790dd24-cf7f-442d-bec7-1a8e7f792a33")

            choose [
                    GET >=> route "/api/players" >=> PlayerApi.list connectionString clubId                                
                    GET >=> route "/api/nils" >-> getNils                                
                                    
                   ] next ctx

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


