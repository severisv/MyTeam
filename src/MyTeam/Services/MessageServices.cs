using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new System.Net.Mail.MailAddress("noreply@wamkam.no", "Wam-Kam FK Web");
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            var transportWeb = new SendGrid.Web(_apiKey);
            return transportWeb.DeliverAsync(myMessage);
          
        }

       
    }
}
