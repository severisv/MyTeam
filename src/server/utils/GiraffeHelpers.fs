namespace MyTeam

open Giraffe
open System.Threading.Tasks
open Microsoft.AspNetCore

[<AutoOpen>]
module GiraffeHelpers =

    type System.Security.Claims.ClaimsPrincipal with
        member user.GetClaim claimType =
            user.Claims
            |> Seq.tryFind (fun c -> c.Type = claimType)
            |> Option.map (fun c -> c.Value)


    let route = routeCi
    let routef = routeCif
    let subRoute = subRouteCi

    type HttpContext = Http.HttpContext

    type Http.HttpContext with
        member ctx.BindJson<'T>() =
            let task = ctx.BindJsonAsync<'T>()

            task
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()


    type Http.HttpContext with
        member ctx.BindForm<'T>() =
            let task = ctx.BindFormAsync<'T>()

            task
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()


    type Http.HttpContext with
        member ctx.TryBindForm<'T>() =
            let task = ctx.TryBindFormAsync<'T>()

            task
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()



module PipelineHelpers =

    let empty: HttpHandler = fun (_: HttpFunc) (_: HttpContext) -> Task.FromResult None


    let removeTrailingSlash next (ctx: HttpContext) =
        let path = ctx.Request.Path.Value

        if path.EndsWith("/") && path.Length > 1 then
            redirectTo true $"%s{path.Remove(path.Length - 1)}%s{ctx.Request.QueryString.Value}" next ctx
        else
            next ctx


    let applyTrollBlock: HttpHandler =
        fun next ctx ->
            if (ctx.Request.Headers["cf-ipcountry"] |> string) = "RU" then
                text "NO WAR 🇺🇦" next ctx
            else
                next ctx


    let (=>) user fn =
        user |> Option.map fn |> Option.defaultValue empty
