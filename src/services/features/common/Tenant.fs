namespace MyTeam

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Users


module Tenant =
    type ClubId = Guid

    type Get = HttpContext -> Option<Club> * Option<User>

    let getClubId (ctx: HttpContext) =
        let hostName = ctx.Request.Host.Value

        let hostNameArray = if hostName.Contains("localhost") then 
                                ["www";"wamkam";".no"]
                            else 
                                hostName.Split('.') |> Seq.toList

        if hostNameArray.Length > 2 then
            if "www".EqualsIc(hostNameArray.[0]) then
                Some(hostNameArray.[1])
            else 
                Some(hostNameArray.[0])
        else 
            None                



    let get : Get =
        fun ctx ->
            let club = 
                getClubId ctx 
                |> Option.bind (fun clubId -> 
                                    let clubQuery () = Clubs.get ctx clubId
                                    
                                    Cache.get ctx ("club-" + clubId) clubQuery
                               )
            let user =      
                club |> Option.bind(fun club -> 
                                    let userId = ctx.User.Identity.Name
                                    let userQuery () = Users.get ctx club.Id userId

                                    Cache.get ctx ("user-" + club.Id.ToString() + userId) userQuery
                    )

            (club, user)                    

