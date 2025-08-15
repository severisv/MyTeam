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

    type BrevoEmailSender = {
        Name: string
        Email: string
    }

    type BrevoEmailRecipient = {
        Name: string option
        Email: string
    }

    type BrevoEmailRequest = {
        Sender: BrevoEmailSender
        To: BrevoEmailRecipient list
        Subject: string
        HtmlContent: string
    }

    let send (serviceProvider: IServiceProvider) (emailAddress: string) subject message =
        let emailAddress = emailAddress.Trim()

        task {
            let apiKey =
                serviceProvider.GetService<IConfiguration>().["Integration:Brevo:Key"]
            
            let logger = Logger.get serviceProvider
            let httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()
            let httpClient = httpClientFactory.CreateClient()

            // Prepare Brevo API request
            let emailRequest = {
                Sender = { Name = "Wam-Kam FK"; Email = "post@wamkam.no" }
                To = [{ Name = None; Email = emailAddress }]
                Subject = subject
                HtmlContent = message
            }

            let json = JsonConvert.SerializeObject(emailRequest)
            let content = new StringContent(json, Encoding.UTF8, "application/json")

            // Set up HTTP client
            httpClient.DefaultRequestHeaders.Add("api-key", apiKey)
            httpClient.DefaultRequestHeaders.Add("accept", "application/json")

            // Send email via Brevo API
            let! response = httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content)
            let! responseContent = response.Content.ReadAsStringAsync()

            if not response.IsSuccessStatusCode then
                failwithf $"Feil ved sending av e-post: ({response.StatusCode}) %s{responseContent}"

            logger.LogInformation $"Sender e-post til %s{emailAddress}. Status: %i{int response.StatusCode}. Response: {responseContent}"
        }


type EmailSender(serviceProvider) =
    member x.SendEmailAsync(email, subject, message) =
        Email.send serviceProvider email subject message
