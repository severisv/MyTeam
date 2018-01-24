namespace MyTeam.Members

open System
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


    let toggleTeam : ToggleTeam =
        fun connectionString clubId memberId teamId -> 
            let (ClubId clubId) = clubId
            let db = Database.get connectionString

            let memb = db.Dbo.Member
                            |> Seq.filter(fun m -> m.Id = memberId)
                            |> Seq.head

            if memb.ClubId <> clubId then failwith "Prøver å redigere spiller fra annen klubb - ingen tilgang"

            let memberTeam = db.Dbo.MemberTeam
                                |> Seq.filter (fun mt -> mt.MemberId = memberId)
                                |> Seq.filter (fun mt -> mt.TeamId = teamId)
                                |> Seq.tryHead

            match memberTeam with 
                | Some m ->
                    m.Delete()
                | None ->
                    let memberTeam = db.Dbo.MemberTeam.Create(memberId, teamId)
                    memberTeam.Id <- Guid.NewGuid()
            
            db.SubmitUpdates()             