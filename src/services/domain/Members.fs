namespace MyTeam.Domain

open System
open Newtonsoft.Json
open Newtonsoft.Json.Converters

type MemberId = Guid

module Members = 

    [<JsonConverter(typedefof<StringEnumConverter>)>]
    type Status =
            | Aktiv = 0
            | Inaktiv = 1
            | Veteran = 2
            | Trener = 3
            | Sluttet = 4 
            
    let toRoleArray (roleString : string) =
        if roleString.Length > 0 then
            roleString.Split [|','|] |> Seq.toList
        else []