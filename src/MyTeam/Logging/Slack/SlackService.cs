using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MyTeam.Logging.Slack
{
    public class SlackService
    {
        private readonly SlackSettings _settings;

        public SlackService(SlackSettings settings)
        {
            _settings = settings;
        }


        public void LogError(string typeName, string message, Exception exception, string environmentName)
        {
            var className = exception?.TargetSite.DeclaringType?.Name ?? typeName;
            var method = exception?.TargetSite.Name;
            var typeNameMethodDelimiter = method != null ? "." : "";

            var stacktrace =
                exception != null
                    ? $"`{exception?.Message}` \n```{exception?.StackTrace}```"
                    : string.Empty;

            using (var client = new HttpClient())
            {
                var payload = new
                {
                    channel = _settings.Channel,
                    username = _settings.Application,
                    icon_emoji = ":warning:",
                    text = $"<!channel>: En feil har oppstått i {environmentName}:exclamation:",
                    attachments = new[]
                    {
                        new
                        {
                            fallback = $"Feil i {_settings.Application} i {environmentName}!",
                            color = "danger",
                            mrkdwn_in = new[] {"fields"},
                            fields = new[]
                            {
                                new
                                {
                                    title = "Message",
                                    value = message,
                                    @short = true
                                },
                                new
                                {
                                    title = "Hvor",
                                    value = $"`{className}{typeNameMethodDelimiter}{method}`",
                                    @short = true
                                },
                                new
                                {
                                    title = "Stacktrace",
                                    value = stacktrace,
                                    @short = false
                                }
                            }
                        }
                    }
                };

                var url = _settings.WebhookUrl;
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                try
                {
                    client.PostAsync(url, content).GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                }
            }
        }

        public void Log(LogLevel logLevel,string name, string message, string environmentName)
        {
            using (var client = new HttpClient())
            {
                var payload = new
                {
                    channel = _settings.Channel,
                    username = _settings.Application,
                    icon_emoji = ":information_source:",
                    text = $"{environmentName}: [{logLevel}] {message}"
                };

                var url = _settings.WebhookUrl;
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                try
                {
                    client.PostAsync(url, content).GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                }
            }
        }
    }

}