namespace Shared.Domain

open Shared
open System

type PlayerStatus =
    | Aktiv
    | Inaktiv
    | Veteran
    | Trener
    | Sluttet


module Members =

    type Status = PlayerStatus

    let statusFromInt =
        function
         | 0 -> Aktiv
         | 1 -> Inaktiv
         | 2 -> Veteran
         | 3 -> Trener
         | _ -> Sluttet

    let statusToInt =
        function
         | Aktiv -> 0
         | Inaktiv -> 1
         | Veteran -> 2
         | Trener -> 3
         | Sluttet -> 4
  
    let fullName (firstName, middleName, lastName) =
        if Strings.hasValue middleName then
            sprintf "%s %s %s" firstName middleName lastName
        else
            sprintf "%s %s" firstName lastName

    type MemberId = System.Guid
    type PlayerId = MemberId
    type UserId = UserId of string

    type Role =
        | Admin
        | Trener
        | Skribent
        | Oppmøte
        | Botsjef
          
    type Member = {
        Id: MemberId
        FacebookId: string
        FirstName: string
        MiddleName: string
        LastName: string
        UrlName: string
        Image: string
        Status: PlayerStatus
    }
    with
        member p.Name = sprintf "%s %s" p.FirstName p.LastName

        member m.FullName = fullName (m.FirstName, m.MiddleName, m.LastName)

    type MemberWithTeamsAndRoles = {
        Details: Member
        Teams: TeamId list
        Roles: Role list
    }

    type User = {
         Id: MemberId
         UserId: string
         FacebookId: string
         FirstName: string
         LastName: string
         UrlName: string
         Image: string
         Roles: Role list
         TeamIds: Guid list
         ProfileIsConfirmed: bool
    }
    with
        member user.Name = sprintf "%s %s" user.FirstName user.LastName
        member user.IsInRole roles = user.Roles |> List.exists (fun role -> roles |> List.contains (role))



    let whenInRole (user: User option) roles fn =
        user |> Option.bind (fun user ->
                    if user.IsInRole roles then Some <| fn user
                    else None
            )
