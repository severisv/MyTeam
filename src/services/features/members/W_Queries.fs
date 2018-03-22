namespace MyTeam.Members

open MyTeam
open MyTeam.Domain
open MyTeam.Domain.Members
open Microsoft.EntityFrameworkCore
open System.Linq

module Queries =

    let members (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Members.Where(fun p -> p.ClubId = clubId)

    let list : ListMembers =
        fun db clubId ->         
            (members db clubId).Include(fun p -> p.MemberTeams) 
            |> Seq.map(fun p ->                            
                            {
                                Details = ({
                                             Id = p.Id
                                             FacebookId = p.FacebookId
                                             FirstName = p.FirstName
                                             MiddleName = p.MiddleName
                                             LastName = p.LastName
                                             Image = p.ImageFull
                                             UrlName = p.UrlName
                                             Status = int p.Status |> enum<Status> 
                                          })
                                Teams = p.MemberTeams 
                                           |> Seq.map(fun team -> team.TeamId)
                                           |> Seq.toList
                                Roles = p.RolesString |> toRoleList
                            }
                    )
            |> Seq.toList                


    let listMembersDetailed: ListMembersDetailed =
        fun db clubId ->
            let (ClubId clubId) = clubId
            query {
                for m in db.Members do
                where (m.ClubId = clubId)
                select (m.Id, m.FirstName, m.MiddleName, m.LastName, m.FacebookId,
                        m.UrlName, m.ImageFull, m.Status, m.BirthDate, m.Phone, m.Email)
                } 
                |> Seq.toList
                |> List.map (fun (id, firstName, middleName, lastName, facebookId,
                                  urlName, image, status, birthDate, phone, email) ->
                        {
                           BirthDate = (birthDate |> toOption)
                           Phone = phone
                           Email = email 
                           Details = 
                            {
                               Id = id
                               FacebookId = facebookId
                               FirstName = firstName
                               MiddleName = middleName
                               LastName = lastName
                               UrlName = urlName
                               Image = image    
                               Status = enum<Status> status    
                            }
                        }
                
                 )
    
    let getFacebookIds : GetFacebookIds =
        fun db clubId ->         
            members db clubId
            |> selectMembers
            |> Seq.map (fun m -> m.FacebookId)
            |> Seq.filter isNotNull
            |> Seq.toList


