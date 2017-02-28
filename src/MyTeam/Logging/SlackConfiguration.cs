using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MyTeam.Logging.Slack;

namespace MyTeam.Logging
{
    public static class SlackConfiguration
    {
       
        public static void AddSlack(this ILoggerFactory loggerFactory, SlackSettings settings, LogLevel logLevel, IHostingEnvironment environment)
        {
            loggerFactory.AddProvider(new SlackLoggerProvider(settings, logLevel, environment));
        }
    }
}