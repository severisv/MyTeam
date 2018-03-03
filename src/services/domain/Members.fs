namespace MyTeam.Domain

open System
open MyTeam
open MyTeam.Enums
open MyTeam.Models.Enums
open Newtonsoft.Json
open Newtonsoft.Json.Converters

type MemberId = Guid
type UserId = UserId of string

[<JsonConverter(typedefof<StringEnumConverter>)>]
type Role =
    | Admin = 0
    | Trener = 1
    | Skribent = 2
    | Oppmøte = 3
    | Botsjef = 4 


module Members = 

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