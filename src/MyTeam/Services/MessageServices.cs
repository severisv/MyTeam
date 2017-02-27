using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyTeam.Services
{

    public class AuthMessageSender : IEmailSender
    {

        private readonly string _apiKey;

        public AuthMessageSender(IConfigurationRoot configuration)
        {
            _apiKey = configuration["Integration:SendGrid:Key"];
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var sg = new SendGridAPIClient(_apiKey);

            var from = new Email("noreply@wamkam.no", "Wam-Kam FK");
            var to = new Email(email);
            var content = new Content("text/html", message);
            var mail = new Mail(from, subject, to, content);

            await sg.client.mail.send.post(requestBody: mail.Get());
        }

     

    }
}
