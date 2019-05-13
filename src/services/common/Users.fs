namespace MyTeam

open System
open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Common.Features.Members
open Microsoft.EntityFrameworkCore
open System.Linq
open Shared.Strings

module Users =
     
    let get : (HttpContext -> ClubId -> UserId -> Option<User>) =
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
                                 UserId = userId
                                 FacebookId = !!m.FacebookId
                                 FirstName = !!m.FirstName
                                 LastName = !!m.LastName
                                 UrlName = !!m.UrlName
                                 Image = !!m.ImageFull
                                 Roles = m.RolesString |> toRoleList
                                 TeamIds = m.MemberTeams 
                                             |> Seq.map(fun team -> team.TeamId)
                                             |> Seq.toList
                                 ProfileIsConfirmed = m.ProfileIsConfirmed
                                })
                |> Seq.tryHead

      