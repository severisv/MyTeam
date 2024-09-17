module Server.ErrorHandling

open Giraffe
open MyTeam
open Shared.Domain.Members
open System
open Server.Common
open Microsoft.Extensions.Logging

let errorHandler (ex: Exception) (logger: Microsoft.Extensions.Logging.ILogger) =
    clearResponse
    >=> setStatusCode 500
    >=> fun next ctx ->
            logger.LogError(
                EventId(),
                ex,
                sprintf "Error in %s: %s . \nReferer: %s" ctx.Request.Method (Request.fullPath ctx) (string ctx.Request.Headers.["Referer"])
            )

            let isDeveloper (user: User) = user.UserId = "severin@sverdvik.no"

            (Tenant.get ctx
             |> function
                 | (Some club, Some user) when user |> isDeveloper ->
                     if Request.isJson ctx then
                         json [ ex.Message; string ex ]
                     else
                         Views.Error.stackTrace club (Some user) ex
                 | (Some club, user) ->
                     if Request.isJson ctx then
                         json [ "500 server error" ]
                     else
                         Views.Error.serverError club user
                 | (None, _) -> text "Error")
                next
                ctx


let logNotFound next (ctx: HttpContext) =
    if not
       <| String.IsNullOrEmpty(ctx.Request.Headers.["Referer"] |> string)
       && ctx.Request.IsHttps
       && [ "crawler"
            "bingbot"
            "Googlebot"
            "SemrushBot"
            "Dataprovider.com"
            "Lynt.cz"
            "DotBot"
            "uptimebot" ]
          |> Seq.exists (
              ctx.Request.Headers.["User-Agent"]
              |> string
              |> contains
          )
          |> not
       && [ "t.co/EEcVe1k3UV"; "binance.com" ]
          |> Seq.exists (
              ctx.Request.Headers.["Referer"]
              |> string
              |> contains
          )
          |> not
       && [ ".php"
            "apple-touch"
            "favicon.ico"
            "index.php"
            "wp"
            "cms"
            "/dev"
            "/tmp"
            ".txt"
            ".zip" ]
          |> Seq.exists (ctx.Request.Path |> string |> contains)
          |> not then
        Logger.get ctx.RequestServices
        |> fun logger ->
            logger.LogWarning(
                sprintf
                    "404: %s Referer: %s  \n %s"
                    (string ctx.Request.Path)
                    (string ctx.Request.Headers.["Referer"])
                    (ctx.Request.Headers
                     |> Seq.filter (fun kv -> not <| kv.Key.ToLower().Contains "cookie")
                     |> Seq.map (fun keyValue -> $"%s{keyValue.Key}: %s{string keyValue.Value}")
                     |> String.concat ",\n  ")
            )

    next ctx
