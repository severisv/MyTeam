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
            

    type MemberWithTeamsAndRoles = {
        Details: Member
        Teams: TeamId list 
        Roles: Role list
    }