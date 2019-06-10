namespace MyTeam.Members

open MyTeam
open Shared
open Shared.Domain
open Shared.Domain.Members
open MyTeam.Common.Features.Members
open Microsoft.EntityFrameworkCore
open System.Linq
open MyTeam.Common.Features.Members


module Queries =

    let members (db : Database) clubId = 
        let (ClubId clubId) = clubId
        db.Members.Where(fun p -> p.ClubId = clubId)

    let listMembersDetailed: ListMembersDetailed =
        fun db clubId ->
            let (ClubId clubId) = clubId
            query {
                for m in db.Members do
                where (m.ClubId = clubId)
                select (m.Id, m.FirstName, m.MiddleName, m.LastName, m.FacebookId,
                        m.UrlName, m.ImageFull, m.Status, m.BirthDate, m.Phone, m.UserName)
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
                               Status = statusFromInt status    
                            }
                        }
                
                 )
                |> List.sortBy (fun p -> p.Details.FirstName)         
