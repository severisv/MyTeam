namespace MyTeam

open System
open Microsoft.AspNetCore.Http
open MyTeam

type UserId = string
type Role = string

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

                    
            let user = database.Dbo.Member 
                        |> Seq.filter(fun m -> m.ClubId = clubId && m.UserName = userId)
                        |> Seq.map(fun m -> 
                                        {
                                         Id = m.Id
                                         FacebookId = m.FacebookId
                                         FirstName = m.FirstName
                                         UrlName = m.UrlName
                                         Image = m.ImageFull
                                         Roles = m.RolesString.Split [|','|] |> Seq.toList
                                         TeamIds = []
                                         ProfileIsConfirmed = m.ProfileIsConfirmed
                                        })
                        |> Seq.tryHead                        

            user            
                                                    

          