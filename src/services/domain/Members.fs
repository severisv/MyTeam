module MyTeam.Domain.Members

open MyTeam
open MyTeam.Enums
open MyTeam.Models.Enums
open System.Linq

type MemberId = Guid
type UserId = UserId of string

type Role =
    | Admin = 0
    | Trener = 1
    | Skribent = 2
    | Oppmøte = 3
    | Botsjef = 4 

type Status = PlayerStatus
        
type Member = {
    Id: MemberId
    FacebookId: string
    FirstName: string
    MiddleName: string
    LastName: string
    UrlName: string
    Image: string    
    Status: Status      
} with
    member p.Name = sprintf "%s %s" p.FirstName p.LastName   

    member m.FullName = 
        sprintf "%s %s%s%s" m.FirstName m.MiddleName (if m.MiddleName.HasValue then " " else "") m.LastName

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