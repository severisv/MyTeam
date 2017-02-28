using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MyTeam.Logging.Slack
{
    public class SlackLogger : ILogger<Type>
    {
        private readonly SlackSettings _settings;
        private readonly IHostingEnvironment _environment;
        private readonly IEnumerable<LogLevel> _enabledLogLevels;
        private readonly string _name;

        public SlackLogger(string name, IHostingEnvironment environment, SlackSettings settings, LogLevel logLevel)
        {
            _name = name;
            _environment = environment;
            _settings = settings;
            _enabledLogLevels = GetEnabledLogLevels(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            var message = FormatLogMessage(state, exception, formatter);

            var slackService = new SlackService(_settings);

            if (logLevel == LogLevel.Error)
            {
                slackService.LogError(_name, message, exception, _environment.EnvironmentName);
            }
            else slackService.Log(logLevel, _name, message, _environment.EnvironmentName);

        }

        private static string FormatLogMessage<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message;
            if (exception == null)
            {
                if (formatter != null)
                {
                    message = formatter(state, null);
                }
                else
                {
                    message = state.ToString();
                }
            }
            else
            {
                message = state + " Message: " + exception.Message;
            }
            return message;
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return 
                !_environment.IsDevelopment() &&
                _enabledLogLevels.Contains(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        private IEnumerable<LogLevel> GetEnabledLogLevels(LogLevel logLevel)
        {
            var result = new List<LogLevel>();
            for (int i = (int)logLevel; i < (int)LogLevel.None; i++)
            {
                result.Add((LogLevel)i);
            }
            return result;
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}