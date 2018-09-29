module MyTeam.Admin.Pages

open Giraffe.GiraffeViewEngine
open MyTeam
open MyTeam.Domain.Member
open MyTeam.Domain.Members
open MyTeam.Views


let index club user ctx =
    [
        mtMain [] [
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
    |> OkResult


let invitePlayers club user ctx =
    [    
        div [
            _id "add-players";
            attr "data-statuses" (Enums.getValues<Status> () |> Json.serialize) 
            attr "data-roles" (Enums.getValues<Role> () |> Json.serialize)                     
            ] []
    
        Admin.coachMenu               
    ] 
    |> layout club user 
                (fun o -> { 
                            o with 
                                Title = "Inviter spillere"
                                Scripts = [ 
                                            FacebookSdk.script ctx
                                            Scripts.documentReady "window.mt_fb.login()" 
                                        ]
                        }
                ) ctx
    |> OkResult