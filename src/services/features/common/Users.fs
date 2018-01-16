namespace MyTeam

open System
open Microsoft.AspNetCore.Http
open MyTeam
open MyTeam.Domain

type UserId = string
type Role = string
type MemberId = Guid

type User = {
     Id: Guid
     FacebookId: string
     FirstName: string
     UrlName: string
     Image: string
     Roles: Role list
     TeamIds: Guid list
     ProfileIsConfirmed: bool
}


module Users =

    type Get = HttpContext -> ClubId -> UserId -> Option<User>
    let get : Get =
        fun ctx clubId userId -> 
            let connectionString = getConnectionString ctx
            let database = Database.get connectionString

            let members = 
                    query {
                        for p in database.Dbo.Member do
                        where (p.ClubId = clubId && p.UserName = userId)
                        join team in database.Dbo.MemberTeam on (p.Id = team.MemberId)
                        select (p, team)
                    }
                    |> Seq.toList

            let teams = members
                        |> Seq.map(fun (__, team) -> team)

                    
            let user = members
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

            user            
                                                    

          