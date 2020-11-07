module Server.Authorization

open Giraffe
open MyTeam
open Shared.Domain.Members
open Common


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


let mustBeInRole (user: Option<User>) (roles: Role list) =
    authorizeUser  (fun __ ->
                            match user with 
                                | Some u -> u.IsInRole roles                                   
                                | None -> false
                        ) accessDenied