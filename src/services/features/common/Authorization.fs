namespace MyTeam

open Giraffe
open MyTeam.Users
open MyTeam.Domain

module Authorization =
    let accessDenied = setStatusCode 403 >=> text "Ingen tilgang"

    let mustBeMember (user: Option<User>) =
        requiresAuthPolicy (fun __ ->
                                user.IsSome
                            ) accessDenied

    let mustBeInRole (user: Option<User>) (roles: Role list) =
        requiresAuthPolicy (fun __ ->
                                match user with 
                                    | Some u -> u.IsInRole roles                                   
                                    | None -> false
                            ) accessDenied