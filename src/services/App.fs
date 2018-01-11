namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open MyTeam
open MyTeam.Common

module App =

    let getNils =
        fun (ctx: HttpContext) ->
            json ["Nils"], ctx

    let webApp =
        fun next ctx ->
            let connectionString = getConnectionString ctx
            let (club, userMember) = Tenant.get ctx
            
            match club with
                | Some club ->
                      choose [
                        GET >=> route "/api/players" >=> PlayerApi.list connectionString club.Id                                
                        GET >=> route "/api/nils" >-> getNils                                
                                        
                       ] next ctx
                | None ->        
                    choose [
                        GET >=> route "/api/players" >=> text "No club"                             
       
                       ] next ctx           
              

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


