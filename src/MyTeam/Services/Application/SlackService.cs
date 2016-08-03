using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace MyTeam.Services.Application
{
    public class SlackService
    {
        private readonly IHostingEnvironment _env;

        public SlackService(IHostingEnvironment env)
        {
            _env = env;
        }
        
        public void Log(Exception exception)
        {
            if (!_env.IsProduction())
            {
                return;
            }

            var message = exception.StackTrace;
            var classname = exception.TargetSite.DeclaringType?.Name;
            var method = exception.TargetSite.Name;
            using (var client = new HttpClient())
            {
                var payload = new
                {
                    channel = "#myteam",
                    username = "wamkam.no",
                    icon_emoji = ":warning:",
                    text = $"<!channel>: En feil har oppstått i {_env.EnvironmentName}:exclamation:",
                    attachments = new[]
                    {
                        new
                        {
                            fallback = "Feil i Selvbetjening i produksjon!",
                            color = "danger",
                            mrkdwn_in = new [] {"fields"},
                            fields = new[]
                            {
                                new
                                {
                                    title = "Message",
                                    value = exception.Message,
                                    @short = true
                                },
                                new
                                {
                                    title = "Metode",
                                    value = $"`{classname}.{method}`",
                                    @short = true
                                },
                                new
                                {
                                    title = "Stacktrace",
                                    value = $"```{message}```",
                                    @short = false
                                }
                            }
                        }
                    }
                };

                const string url = "https://hooks.slack.com/services/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";
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;
              
            }
        }
    }
}
