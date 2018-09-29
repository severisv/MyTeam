namespace MyTeam.Domain

open System

type PlayerStatus =
    | Aktiv = 0
    | Inaktiv = 1
    | Veteran = 2
    | Trener = 3
    | Sluttet = 4

 
module Member = 

    type Status = PlayerStatus

    let fullName (firstName, middleName, lastName) =
        sprintf "%s %s%s%s" firstName middleName (if not (String.IsNullOrEmpty(middleName)) then " " else "") lastName


