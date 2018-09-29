namespace MyTeam.Domain

open System

type PlayerStatus =
    | Aktiv = 0
    | Inaktiv = 1
    | Veteran = 2
    | Trener = 3
    | Sluttet = 4

 
  
module Members = 
  
    type Status = PlayerStatus

    let fullName (firstName, middleName, lastName) =
        sprintf "%s %s%s%s" firstName middleName (if not (String.IsNullOrEmpty(middleName)) then " " else "") lastName


  
    let parse<'T> value = 
        try
            Enum.Parse(typeof<'T>, value, true)  :?> 'T
        with
        | :? ArgumentException -> failwithf "Ugyldig verdi for Enum %s: '%s'" typeof<'T>.FullName value             
               


    type MemberId = System.Guid
    type PlayerId = MemberId
    type UserId = UserId of string

    type Role =
        | Admin = 0
        | Trener = 1
        | Skribent = 2
        | Oppmøte = 3
        | Botsjef = 4 

        

        

            
    type Member = {
        Id: MemberId
        FacebookId: string
        FirstName: string
        MiddleName: string
        LastName: string
        UrlName: string
        Image: string    
        Status: PlayerStatus      
    } with
        member p.Name = sprintf "%s %s" p.FirstName p.LastName   

        member m.FullName = fullName (m.FirstName, m.MiddleName, m.LastName)
            

    let toRoleList (roleString : string) =
        if not <| isNull roleString && roleString.Length > 0 then
            roleString.Split [|','|] 
            |> Seq.map(parse<Role>)
            |> Seq.toList
        else []

    let fromRoleList (roles: Role list) = System.String.Join(",", roles)

