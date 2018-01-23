namespace MyTeam.Members

open MyTeam
open MyTeam.Domain

module Persistence =
    let setStatus : SetStatus =
        fun connectionString clubId memberId status -> 
            let (ClubId clubId) = clubId
            let (members, db) = Queries.members connectionString clubId
            members
            |> Seq.filter(fun p -> p.Id = memberId)
            |> Seq.iter(fun p ->
                p.Status <- int status
            )
            db.SubmitUpdates()


    let toggleRole : ToggleRole =
        fun connectionString clubId memberId role ->

            let toggleRoleInList role roleList =
                if roleList |> List.contains role then
                    roleList |> List.filter (fun r -> r = role)
                else 
                    roleList |> List.append [role]      

            let (ClubId clubId) = clubId
            let (members, db) = Queries.members connectionString clubId
            members
            |> Seq.filter(fun p -> p.Id = memberId)
            |> Seq.iter(fun p ->
                p.RolesString <- p.RolesString 
                                |> Members.toRoleList
                                |> toggleRoleInList role
                                |> Members.fromRoleList
                       )
            db.SubmitUpdates()        

