namespace MyTeam

open System
open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore
open System.Linq

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
                let db = ctx.Database

                let (ClubId clubId) = clubId
                                   
                db.Members.Include(fun m -> m.MemberTeams).Where(fun m -> m.UserName = userId && m.ClubId = clubId)
                |> Seq.map(fun m -> 
                                {
                                 Id = m.Id
                                 FacebookId = m.FacebookId
                                 FirstName = m.FirstName
                                 LastName = m.LastName
                                 UrlName = m.UrlName
                                 Image = m.ImageFull
                                 Roles = m.RolesString |> Members.toRoleList
                                 TeamIds = m.MemberTeams 
                                             |> Seq.map(fun team -> team.TeamId)
                                             |> Seq.toList
                                 ProfileIsConfirmed = m.ProfileIsConfirmed
                                })
                |> Seq.tryHead                            
                                                    
                                                

      