namespace MyTeam.Members

open MyTeam
open MyTeam.Domain

module Persistence =
    let setStatus : SetStatus =
        fun connectionString clubId memberId status -> 
            let (ClubId clubId) = clubId
            let (members, db) = Queries.members connectionString clubId
            let memb = members
                    |> Seq.filter(fun p -> p.Id = memberId)
                    |> Seq.head

            memb.Status <- int status
            db.SubmitUpdates()
            UserId memb.UserName


    let toggleRole : ToggleRole =
        fun connectionString clubId memberId role ->

            let toggleRoleInList role roleList =
                if roleList |> List.contains role then
                    roleList |> List.filter (fun r -> r <> role)
                else 
                    roleList |> List.append [role]      

            let (ClubId clubId) = clubId
            let (members, db) = Queries.members connectionString clubId
            
            let memb = members
                    |> Seq.filter(fun p -> p.Id = memberId)
                    |> Seq.head

            memb.RolesString <- memb.RolesString 
                                |> Members.toRoleList
                                |> toggleRoleInList role
                                |> Members.fromRoleList
                       
            db.SubmitUpdates()        
            UserId memb.UserName

