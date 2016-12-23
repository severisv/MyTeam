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

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Send(email, subject, message);
          
        }

        Task Send(string email, string subject, string message)
        {
            dynamic sg = new SendGridAPIClient(_apiKey);

            var from = new Email("noreply@wamkam.no", "Wam-Kam FK Web");
            var to = new Email(email);
            var content = new Content("text/html", message);
            var mail = new Mail(from, subject, to, content);

            return sg.client.mail.send.post(requestBody: mail.Get());
        }


    }
}
