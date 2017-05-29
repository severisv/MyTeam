using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyTeam.Services
{

    public class AuthMessageSender : IEmailSender
    {

        private readonly string _apiKey;
        private readonly ILogger _logger;

        public AuthMessageSender(IConfigurationRoot configuration, ILogger<AuthMessageSender> logger)
        {
            _apiKey = configuration["Integration:SendGrid:Key"];
            _logger = logger;

        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("noreply@wamkam.no", "Wam-Kam FK");
            var to = new EmailAddress(email);
            
            var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent: message);

            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation($"Sender e-post til {email}. Status: {response.StatusCode}");

        }

     

    }
}
