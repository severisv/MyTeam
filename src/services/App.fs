namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open MyTeam
open MyTeam.Common

module App =

    let webApp =
        fun next ctx ->
            let (club, userMember) = Tenant.get ctx
            
            match club with
                | Some club ->
                      choose [
                        GET >=> route "/api/players" >-> PlayerApi.list club                                
                        GET >=> route "/api/players/facebookids" >-> PlayerApi.getFacebookIds club                                
                                        
                       ] next ctx
                | None ->        
                    choose [
                        GET >=> route "/api/players" >=> text "No club"                             
       
                       ] next ctx           
              

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


