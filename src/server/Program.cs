using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SlackLogger;

namespace MyTeam
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Set up configuration sources.
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                })

                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddSlack(options =>
                    {
                        options.WebhookUrl =
                            "https://hooks.slack.com/services/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";
                        options.LogLevel = hostingContext.HostingEnvironment.IsDevelopment() ? LogLevel.None : LogLevel.Information;
                        options.Channel = "#myteam";
                    });
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

