module MyTeam.Program

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open SlackLogger
open MyTeam.Startup


let configureAppsettings (ctx: WebHostBuilderContext) (config: IConfigurationBuilder) =
    config.SetBasePath(ctx.HostingEnvironment.ContentRootPath).AddJsonFile("appsettings.json")
          .AddJsonFile(sprintf "appsettings.%s.json" ctx.HostingEnvironment.EnvironmentName, true, true)
          .AddEnvironmentVariables()
    |> ignore

let configureLogging (ctx: WebHostBuilderContext) (logging: ILoggingBuilder) =
    logging.AddConfiguration(ctx.Configuration.GetSection("Logging")).AddConsole().AddDebug().AddSlack()
    |> ignore



[<EntryPoint>]
let main _ =
        WebHostBuilder()
            .UseKestrel()
            .UseIISIntegration()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureAppsettings)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .Configure(configureApp)
            .Build()
            .Run()
        
        0
