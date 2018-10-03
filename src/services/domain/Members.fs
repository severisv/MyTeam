namespace MyTeam.Domain

open MyTeam
open MyTeam.Enums
open System.Linq



open Members
module Memberqueries =

    let toRoleList (roleString : string) =
        if not <| isNull roleString && roleString.Length > 0 then
            roleString.Split [|','|] 
            |> Seq.map(parse<Role>)
            |> Seq.toList
        else []

    let fromRoleList (roles: Role list) = System.String.Join(",", roles)
    
    let selectMembers =
            fun (players: IQueryable<Models.Domain.Member>) ->
                    query {
                        for p in players do
                        sortBy p.FirstName
                        select (p.Id, p.FacebookId, p.FirstName, p.MiddleName, p.LastName, p.UrlName, p.ImageFull, p.Status)
                    }
                    |> Seq.map 
                        (fun (id, facebookId, firstName, middleName, lastName, urlName, imageFull, status) ->
                            {
                                Id = id
                                FacebookId = facebookId
                                FirstName = firstName
                                MiddleName = middleName
                                LastName = lastName
                                UrlName = urlName
                                Image = imageFull
                                Status = enum<PlayerStatus> status
                             })