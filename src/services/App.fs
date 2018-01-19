namespace MyTeam

open Giraffe
open Microsoft.AspNetCore.Builder
open MyTeam
open MyTeam.Authorization

module App =

    let webApp =
        fun next ctx ->
            let (club, user) = Tenant.get ctx

            match club with
                | Some club ->
                      choose [
                        GET >=> route "/api/players" >-> PlayerApi.list club.Id
                        GET >=> route "/api/players/facebookids" >-> PlayerApi.getFacebookIds club.Id
                        PUT >=> mustBeMember user >=> routef "/api/players/%s/status" (parseGuid >> PlayerApi.setStatus)

                       ] next ctx
                | None ->
                    choose [
                        GET >=> route "/api/players" >=> text "No club"

                       ] next ctx


    let useGiraffe (app : IApplicationBuilder)  =
            app.UseGiraffe webApp


