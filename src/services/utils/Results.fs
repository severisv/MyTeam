namespace MyTeam

open Shared

type HttpResult<'a> =
    | OkResult of 'a
    | Redirect of string
    | ValidationErrors of list<ValidationError>
    | Unauthorized
    | NotFound
