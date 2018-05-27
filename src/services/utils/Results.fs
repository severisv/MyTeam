namespace MyTeam

type ValidationError = {
    Name: string
    Errors: string list
}

type Error = 
    | ValidationErrors of list<ValidationError>            
    | Redirect of string
    | Unauthorized            
    | NotFound   
