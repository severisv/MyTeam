namespace Services.Utils

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open SendGrid
open SendGrid.Helpers.Mail
open MyTeam
open FSharp.Control.Tasks.V2.ContextInsensitive



module Email =

    let send (serviceProvider : IServiceProvider) emailAddress subject message =

        task {
            let apiKey = serviceProvider.GetService<IConfiguration>().["Integration:SendGrid:Key"]
            let logger = Logger.get serviceProvider
    
            let client = SendGridClient(apiKey);
    
            let message =
                MailHelper.CreateSingleEmail
                    ( from = EmailAddress("noreply@wamkam.no", "Wam-Kam FK"),
                      ``to`` = EmailAddress emailAddress,
                      subject = subject,
                      plainTextContent = "",
                      htmlContent = message )
    
            let! response = client.SendEmailAsync message
            let! content = response.Body.ReadAsStringAsync()
            if (int response.StatusCode) >= 300 then failwithf "Feil ved sending av e-post: (%O) %s" response.StatusCode content 
               
            logger.LogInformation (sprintf "Sender e-post til %s. Status: %i. Message: %O" emailAddress (int response.StatusCode) message)
        }       
       
       
type EmailSender(serviceProvider) =
    member x.SendEmailAsync(email, subject, message) =
        Email.send serviceProvider email subject message
