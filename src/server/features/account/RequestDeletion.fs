module MyTeam.Account.RequestDeletion

open MyTeam
open System
open System.Text
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open MyTeam.Views.LayoutComponents
open Giraffe.ViewEngine.HtmlElements
open Giraffe.ViewEngine.Attributes
open Shared.Domain
open Giraffe.Core
open System.IO

let requestDeletion: HttpHandler =
    fun next (ctx: HttpContext) ->

        let logger = Logger.get ctx.RequestServices


        let userId =
            try
                let signed_request =
                    ctx.Request.Form["signed_request"] |> string

                let fbUser =
                    $"{signed_request.Split('.')[1]}="
                    |> Convert.FromBase64String
                    |> Encoding.UTF8.GetString
                    |> JsonConvert.DeserializeObject<{| user_id: string |}>

                fbUser.user_id
            with
            | e ->
                logger.LogError(EventId(), e, "Klarte ikke parse signed_request")

                Guid.NewGuid() |> string

        logger.LogInformation(EventId(3), $"User {userId} requested deletion.")

        json
            {| url = $"{ctx.Request.Scheme}://{ctx.Request.Host}/konto/sletting/{userId}"
               confirmation_code = userId |}
            next
            ctx




let showStatus (club: Club) user userId (ctx: HttpContext) =

    let db = ctx.Database

    let u =
        query {
            for m in db.Members do
                where (m.FacebookId = userId)
                select (m.FirstName, m.LastName)
        }

        |> Seq.toList
        |> List.map (fun (firstName, lastName) ->

            {| FirstName = firstName
               LastName = lastName |})
        |> List.tryHead

    let name =
        u
        |> Option.map (fun u -> u.FirstName + " " + u.LastName)
        |> Option.defaultValue userId

    [ mtMain [] [

          div [ _class "mt-container" ] [
              p [ _style "font-weight: bold;" ] [
                  encodedText $"Delete status for user {name}"
              ]
              p [] [
                  encodedText
                      $"""Status: {u
                                   |> Option.map (fun _ -> "Pending")
                                   |> Option.defaultValue "Completed"}"""
              ]
          ]
      ] ]
    |> layout club user (fun o -> { o with Title = "Sletting" }) ctx
    |> OkResult
