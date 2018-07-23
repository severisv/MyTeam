namespace MyTeam

type ValidationError = {
    Name: string
    Errors: string list
}

type HttpResult<'a> =
    | OkResult of 'a
    | Redirect of string
    | ValidationErrors of list<ValidationError>            
    | Unauthorized            
    | NotFound   
