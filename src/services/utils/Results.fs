namespace MyTeam

open Shared

type HttpResult<'a> =
    | OkResult of 'a
    | Redirect of string
    | ValidationErrors of list<ValidationError>
    | Unauthorized
    | NotFound

module HttpResult =
    let map fn =
        function
        | OkResult a -> OkResult <| fn a
        | Redirect s -> Redirect s
        | ValidationErrors e -> ValidationErrors e
        | Unauthorized -> Unauthorized
        | NotFound -> NotFound

    let fromOption fn =
        function 
        | Some r -> OkResult <| fn r 
        | None -> NotFound   