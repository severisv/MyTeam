namespace MyTeam

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members

module Users =
     
    type UserId = string

    type User = {
         Id: MemberId
         FacebookId: string
         FirstName: string
         UrlName: string
         Image: string
         Roles: Role list
         TeamIds: Guid list
         ProfileIsConfirmed: bool
    }

    type Get = HttpContext -> ClubId -> UserId -> Option<User>
    let get : Get =
        fun ctx clubId userId -> 
            if String.IsNullOrEmpty(userId) then
                None
            else            
                let connectionString = getConnectionString ctx
                let database = Database.get connectionString

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
                                 UrlName = m.UrlName
                                 Image = m.ImageFull
                                 Roles = m.RolesString |> Members.toRoleArray
                                 TeamIds = teams |> Seq.filter(fun team -> team.MemberId = m.Id) 
                                                 |> Seq.map(fun team -> team.TeamId)
                                                 |> Seq.toList
                                 ProfileIsConfirmed = m.ProfileIsConfirmed
                                })
                |> Seq.tryHead                            
                                                    
                                                

      