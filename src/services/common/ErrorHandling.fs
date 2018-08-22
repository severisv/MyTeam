module MyTeam.ErrorHandling

open Giraffe
open MyTeam
open System
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting


let errorHandler (ex : Exception) (logger : Microsoft.Extensions.Logging.ILogger) =
        clearResponse
        >=> setStatusCode 500
        >=> fun next ctx -> 
                logger.LogError(EventId(), 
                                ex, 
                                sprintf "Error in %s: %s" 
                                            ctx.Request.Method 
                                            (Request.fullPath ctx)
                                )

                let isDeveloper (user: Users.User) = user.UserId = "severin@sverdvik.no"

                (Tenant.get ctx 
                    |> function
                    | (Some club, Some user) when user |> isDeveloper -> 
                        if Request.isJson ctx then
                            json [ex.Message; string ex]
                        else
                            Views.Error.stackTrace club (Some user) ex
                    | (Some club, user) -> 
                        if Request.isJson ctx then 
                            json ["500 server error"]
                        else 
                            Views.Error.serverError club user
                    | (None, _) -> text "Error"
                ) next ctx


let logNotFound next ctx =
    Logger.get ctx 
    |> fun logger ->
        logger.LogWarning(sprintf "404: %s Referer: %s " (string ctx.Request.Path) (string ctx.Request.Headers.["Referer"]) )
    next ctx