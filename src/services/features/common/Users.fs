namespace MyTeam

open System
open MyTeam
open MyTeam.Domain

module Users =
     
    type User = {
         Id: MemberId
         FacebookId: string
         FirstName: string
         LastName: string
         UrlName: string
         Image: string
         Roles: Role list
         TeamIds: Guid list
         ProfileIsConfirmed: bool
    } with 
        member user.Name = sprintf "%s %s" user.FirstName user.LastName
        member user.IsInRole roles = user.Roles |> List.exists(fun role -> roles |> List.contains(role))

    type Get = HttpContext -> ClubId -> UserId -> Option<User>
    let get : Get =
        fun ctx clubId userId -> 
            let (UserId userId) = userId
            if String.IsNullOrEmpty(userId) then
                None
            else            
                let connectionString = getConnectionString ctx
                let database = Db.get connectionString

                let (ClubId clubId) = clubId
                let members = 
                        query {
                            for p in database.Dbo.Member do
                            where (p.ClubId = clubId)
                            where (p.UserName = userId)
                            join team in database.Dbo.MemberTeam on (p.Id = team.MemberId)
                            select (p, team)
                        }

                let teams = members
                            |> Seq.map(fun (__, team) -> team)


                        
                members
                |> Seq.map(fun (m, __) -> 
                                {
                                 Id = m.Id
                                 FacebookId = m.FacebookId
                                 FirstName = m.FirstName
                                 LastName = m.LastName
                                 UrlName = m.UrlName
                                 Image = m.ImageFull
                                 Roles = m.RolesString |> Members.toRoleList
                                 TeamIds = teams |> Seq.filter(fun team -> team.MemberId = m.Id) 
                                                 |> Seq.map(fun team -> team.TeamId)
                                                 |> Seq.toList
                                 ProfileIsConfirmed = m.ProfileIsConfirmed
                                })
                |> Seq.tryHead                            
                                                    
                                                

      