using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using MyTeam.Services.Application;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MyTeam.Services
{

    public class AuthMessageSender : IEmailSender
    {

        private readonly string _apiKey;
        private readonly IHostingEnvironment _env;

        public AuthMessageSender(IConfigurationRoot configuration, IHostingEnvironment env)
        {
            _apiKey = configuration["Integration:SendGrid:Key"];
            _env = env;

        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("noreply@wamkam.no", "Wam-Kam FK");
            var to = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, htmlContent: message);

            var response = await client.SendEmailAsync(msg);

            var slackService = new SlackService(_env);
            slackService.Log(new Exception(response.StatusCode.ToString()), "empty path");
        }

     

    }
}
