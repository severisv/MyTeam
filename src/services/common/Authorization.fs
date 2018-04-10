namespace MyTeam

open Giraffe
open MyTeam.Users
open MyTeam.Domain.Members

module Authorization =
    let accessDenied = 
        fun next (ctx: HttpContext) ->
            (if not ctx.User.Identity.IsAuthenticated then
                redirectTo false 
                <| sprintf "/konto/innlogging?returnUrl=%s" (Request.fullPath ctx)
            else            
                setStatusCode 403 >=> 
                (Tenant.get ctx 
                    |> function
                    | (Some club, user) -> 
                        if Request.isJson ctx then 
                            json ["403 unauthorized"]
                        else 
                            Views.Error.unauthorized club user
                    | (None, _) -> text "Unauthorized"
                ) ) 
                next ctx

    let mustBeMember = 
        fun next ctx -> requiresAuthentication accessDenied next ctx

    let mustBeInRole (user: Option<User>) (roles: Role list) =
        requiresAuthPolicy (fun __ ->
                                match user with 
                                    | Some u -> u.IsInRole roles                                   
                                    | None -> false
                            ) accessDenied