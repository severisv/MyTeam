namespace MyTeam

open Giraffe
open MyTeam.Users

module Authorization =
    let mustBeMember (user: Option<User>) =
        requiresAuthPolicy (fun __ ->
                                user.IsSome
                            )
                            (setStatusCode 401 >> text "Ingen tilgang")