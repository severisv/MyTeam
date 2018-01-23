namespace MyTeam.Domain

open System
open MyTeam.Enums
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

    [<JsonConverter(typedefof<StringEnumConverter>)>]
    type Status =
        | Aktiv = 0
        | Inaktiv = 1
        | Veteran = 2
        | Trener = 3
        | Sluttet = 4 
            

    let toRoleList (roleString : string) =
        if roleString.Length > 0 then
            roleString.Split [|','|] 
            |> Seq.map(parse<Role>)
            |> Seq.toList
        else []

    let fromRoleList (roles: Role list) = System.String.Join(",", roles)