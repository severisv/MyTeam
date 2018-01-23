namespace MyTeam

open MyTeam
open MyTeam.Domain
open MyTeam.Users


module Tenant =
    type Get = HttpContext -> Option<Club> * Option<User>
    type ClearUserCache = HttpContext -> ClubId -> UserId -> unit
   
    let clubKey club =
        let (ClubIdentifier club) = club 
        "club-" + club.ToString()

    let memberKey clubId userId =
        let (ClubId clubId) = clubId
        let (UserId userId) = userId
        "user-" + clubId.ToString() + userId.ToString()   


    let get : Get =
        let getClubId (ctx: HttpContext) =
            let hostName = ctx.Request.Host.Value

            let hostNameArray = if hostName.Contains("localhost") then 
                                    ["www";"wamkam";".no"]
                                else 
                                    hostName.Split('.') |> Seq.toList

            if hostNameArray.Length > 2 then
                if "www".EqualsIc(hostNameArray.[0]) then
                    Some(ClubIdentifier hostNameArray.[1])
                else 
                    Some(ClubIdentifier hostNameArray.[0])
            else 
                None                

        fun ctx ->
            let club = 
                getClubId ctx 
                |> Option.bind (fun clubId -> 
                                    let clubQuery () = Clubs.get ctx clubId
                                    
                                    Cache.get ctx (clubKey clubId) clubQuery
                               )
            let user =      
                club |> Option.bind(fun club -> 
                                    let userId = (UserId ctx.User.Identity.Name)
                                    let userQuery () = Users.get ctx club.Id userId

                                    Cache.get ctx (memberKey club.Id userId) userQuery
                    )

            (club, user)                    


    let clearUserCache : ClearUserCache = 
        fun ctx clubId userId -> 
            Cache.clear ctx (memberKey clubId userId)
            Cache.clear ctx ("old-" + (memberKey clubId userId))
            