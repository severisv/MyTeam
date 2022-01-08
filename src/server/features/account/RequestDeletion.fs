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

let requestDeletion (ctx: HttpContext) _ =

    let userId = try
                    let signed_request = ctx.Request.Form["signed_request"] |> string
                    let split = signed_request.Split('.');
                    let dataRaw = split[1]

                    let dataBuffer = Convert.FromBase64String(dataRaw)
                    let json = Encoding.UTF8.GetString(dataBuffer);
                    let fbUser = JsonConvert.DeserializeObject<{| user_id: string |}>(json)
                    fbUser.user_id
                 with e ->
                    e.Message.Substring(0,22)
                    

    let logger = Logger.get ctx.RequestServices

    logger.LogInformation(EventId(3),
                          $"User {userId} requested deletion."
                        )

    OkResult

        {| url = $"{ctx.Request.Scheme}://{ctx.Request.Host}/konto/sletting/{userId}"
           confirmation_code = userId |}


let showStatus (club: Club) user  userId (ctx: HttpContext) =

        [ mtMain [] [

                  div [ _class "mt-container" ] [
                      p [_style "font-weight: bold;"] [encodedText $"Delete status for user {userId}"]
                      p [] [encodedText "Status: Deleted"]
                   ]
        ]
        ]
        |> layout club user (fun o -> { o with Title = "Sletting" }) ctx
        |> OkResult