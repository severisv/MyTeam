namespace Services.Utils

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open SendGrid
open SendGrid.Helpers.Mail
open MyTeam



module Email =

    let send (serviceProvider: IServiceProvider) (emailAddress: string) subject message =
        let emailAddress = emailAddress.Trim()

        task {
            let apiKey =
                serviceProvider.GetService<IConfiguration>().["Integration:SendGrid:Key"]

            let logger = Logger.get serviceProvider

            let client = SendGridClient(apiKey)

            let message =
                MailHelper.CreateSingleEmail(
                    from = EmailAddress("noreply@wamkam.no", "Wam-Kam FK"),
                    ``to`` = EmailAddress(emailAddress),
                    subject = subject,
                    plainTextContent = "",
                    htmlContent = message
                )

            let! response = client.SendEmailAsync message
            let! content = response.Body.ReadAsStringAsync()

            if (int response.StatusCode) >= 300 then
                failwithf $"Feil ved sending av e-post: ({response.StatusCode}) %s{content}"

            logger.LogInformation $"Sender e-post til %s{emailAddress}. Status: %i{int response.StatusCode}. Message: {message}"
        }


type EmailSender(serviceProvider) =
    member x.SendEmailAsync(email, subject, message) =
        Email.send serviceProvider email subject message
