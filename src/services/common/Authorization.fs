namespace MyTeam

open Giraffe
open MyTeam.Users
open MyTeam.Domain.Members

module Authorization =
    let accessDenied = setStatusCode 403 >=> text "Ingen tilgang"

    let mustBeMember = 
        fun next ctx -> requiresAuthentication accessDenied next ctx

    let mustBeInRole (user: Option<User>) (roles: Role list) =
        requiresAuthPolicy (fun __ ->
                                match user with 
                                    | Some u -> u.IsInRole roles                                   
                                    | None -> false
                            ) accessDenied