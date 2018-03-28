namespace MyTeam

open MyTeam
open MyTeam.Domain.Members
open MyTeam.Domain
open MyTeam.Users


module Tenant =
    type Get = HttpContext -> Option<Club> * Option<User>
    type ClearUserCache = HttpContext -> ClubId -> UserId -> unit
   
    let clubKey club =
        let (ClubIdentifier club) = club 
        "club-" + string club

    let memberKey clubId userId =
        let (ClubId clubId) = clubId
        let (UserId userId) = userId
        "user-" + string clubId + string userId 

    let get : Get =
        let getClubId (ctx: HttpContext) =

            let hostName = ctx.Request.Host.Value

            let hostNameArray = if hostName.Contains("localhost") then ["www";"wamkam";"no"]
                                else hostName.Split('.') 
                                    |> Seq.toList
                                    |> List.map toLower
                                |> List.filter (fun p -> p <> "www")



            if hostNameArray.Length > 1 then
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
                if ctx.User.Identity.IsAuthenticated then      
                    club |> Option.bind(fun club -> 
                                        let userId = (UserId ctx.User.Identity.Name )
                                        let userQuery () = Users.get ctx club.Id userId

                                        Cache.get ctx (memberKey club.Id userId) userQuery
                        )
                else None                    

            (club, user)                    


    let clearUserCache : ClearUserCache = 
        fun ctx clubId userId -> 
            Cache.clear ctx (memberKey clubId userId)
            Cache.clear ctx ("old-" + (memberKey clubId userId))
            