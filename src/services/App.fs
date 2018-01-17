namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open MyTeam
open MyTeam.Common

module App =

    let mustBeMember (user: Option<User>) = 
        requiresAuthPolicy (fun __ -> 
                                user.IsSome
                            ) 
                            (setStatusCode 401 >> text "Ingen tilgang")
 
    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx
            
            match club with
                | Some club ->
                      choose [
                        GET >=> route "/api/players" >-> PlayerApi.list club                                
                        GET >=> route "/api/players/facebookids" >-> PlayerApi.getFacebookIds club                                
                        GET >=> route "/api/players/status"  >=> mustBeMember user >=> text "hi"                                
                                        
                       ] next ctx
                | None ->        
                    choose [
                        GET >=> route "/api/players" >=> text "No club"                             
       
                       ] next ctx           
              

    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


