module MyTeam.Admin.Pages


open Giraffe
open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open MyTeam.Views
open MyTeam.Attendance.Queries
open MyTeam.Attendance
open MyTeam.Ajax


let index (club: Club) user (ctx: HttpContext) =
    [
        main [] [
            block [] [
                div [
                    _id "manage-players";
                    attr "data-statuses" (Enums.getValues<Status> () |> Json.serialize) 
                    attr "data-roles" (Enums.getValues<Role> () |> Json.serialize)                     
                    ] []
            ]
        ]
        Admin.coachMenu               
    ] 
    |> layout club user (fun o -> { o with Title = "Administrer spillere" }) ctx
    |> Ok
