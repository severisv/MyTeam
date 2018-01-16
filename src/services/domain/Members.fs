namespace MyTeam.Domain

module Members = 
    let toRoleArray (roleString : string) =
        if roleString.Length > 0 then
            roleString.Split [|','|] |> Seq.toList
        else []