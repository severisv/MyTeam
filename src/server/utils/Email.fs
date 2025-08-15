namespace Services.Utils

open System
open System.Net.Http
open System.Text
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open MyTeam



module Email =

    [<JsonObject(NamingStrategyType = typeof<Newtonsoft.Json.Serialization.CamelCaseNamingStrategy>)>]
    type ResendEmailRequest = {
        [<JsonProperty("from")>]
        From: string
        [<JsonProperty("to")>]
        To: string list
        [<JsonProperty("subject")>]
        Subject: string
        [<JsonProperty("html")>]
        Html: string
    }

    let send (serviceProvider: IServiceProvider) (emailAddress: string) subject message =
        let emailAddress = emailAddress.Trim()

        task {
            let apiKey =
                serviceProvider.GetService<IConfiguration>().["Integration:Resend:Key"]
            
            let logger = Logger.get serviceProvider
            let httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()
            let httpClient = httpClientFactory.CreateClient()

            // Prepare Resend API request
            let emailRequest = {
                From = "Wam-Kam FK <post@wamkam.no>"
                To = [emailAddress]
                Subject = subject
                Html = message
            }

            let json = JsonConvert.SerializeObject(emailRequest)
            logger.LogInformation $"Sending email JSON: {json}"
            let content = new StringContent(json, Encoding.UTF8, "application/json")

            // Set up HTTP client headers for Resend
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}")
            httpClient.DefaultRequestHeaders.Add("User-Agent", "MyTeam/1.0")

            // Send email via Resend API
            let! response = httpClient.PostAsync("https://api.resend.com/emails", content)
            let! responseContent = response.Content.ReadAsStringAsync()

            if not response.IsSuccessStatusCode then
                failwithf $"Feil ved sending av e-post: ({response.StatusCode}) %s{responseContent}"

            logger.LogInformation $"Sender e-post til %s{emailAddress}. Status: %i{int response.StatusCode}. Response: {responseContent}"
        }


type EmailSender(serviceProvider) =
    member x.SendEmailAsync(email, subject, message) =
        Email.send serviceProvider email subject message
