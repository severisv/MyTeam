using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MyTeam.Logging.Slack
{
    public class SlackLoggerProvider : ILoggerProvider
    {

        private readonly SlackSettings _settings;
        private readonly LogLevel _logLevel;
        private readonly IHostingEnvironment _environment;

        public SlackLoggerProvider(SlackSettings settings, LogLevel logLevel, IHostingEnvironment environment)
        {
            _settings = settings;
            _logLevel = logLevel;
            _environment = environment;
        }

        public ILogger CreateLogger(string name)
        {
            return new SlackLogger(name, _environment, _settings, _logLevel);
        }
        
        public void Dispose()
        {
        }
    }
}