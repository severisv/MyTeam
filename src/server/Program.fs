module MyTeam.Program

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open SlackLogger
open Microsoft.AspNetCore.Builder;
open Microsoft.EntityFrameworkCore;
open Microsoft.Extensions.DependencyInjection;
open MyTeam.Models;
open Microsoft.AspNetCore.Identity;
open Microsoft.AspNetCore.Authentication.Cookies;
open Services.Utils;
open Giraffe
open Giraffe.Serialization
open Newtonsoft.Json
open Newtonsoft.Json.Converters

let configureServices (services: IServiceCollection) =
        let configuration = services.BuildServiceProvider().GetService<IConfiguration>()
        services.AddDbContext<ApplicationDbContext>(fun options ->
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")) |> ignore) |> ignore

        services
            .AddMemoryCache()
            .AddAntiforgery()
            .AddGiraffe()
            .AddAuthorization()
            .AddIdentity<ApplicationUser, IdentityRole>(fun o ->            
                o.Password.RequireDigit <- false;
                o.Password.RequireUppercase <- false;
                o.Password.RequireNonAlphanumeric <- false;
                o.Password.RequiredLength <- 7;
            )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders() |> ignore

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(fun o ->
                
                    o.Cookie.Expiration <- Nullable <| TimeSpan.FromDays(16.0);
                    o.Cookie.Name <- "_myt";
                    o.SlidingExpiration <- true;
                
            )
            .AddFacebook(fun o ->            
                o.AppId <- configuration.["Authentication:Facebook:AppId"];
                o.AppSecret <- configuration.["Authentication:Facebook:AppSecret"];
            ) |> ignore

        services.Configure<CloudinarySettings>(configuration.GetSection("Integration:Cloudinary"))
            .Configure<FacebookOptions>(configuration.GetSection("Authentication:Facebook"))
            .Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"))
            .Configure<AssetHashes>(configuration.GetSection("AssetHashes"))       
            .AddApplicationInsightsTelemetry()
             |> ignore
        

        services.AddTransient<EmailSender>() |> ignore

        let settings = JsonSerializerSettings ()
        settings.ContractResolver <- Serialization.CamelCasePropertyNamesContractResolver()   
        settings.Converters.Add(OptionConverter())
        settings.Converters.Add(IdiomaticDuConverter())
        settings.Converters.Add(StringEnumConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer(settings)) |> ignore

        
        


let configureApp (app: IApplicationBuilder) =
        let env = app.ApplicationServices.GetService<IWebHostEnvironment>()        
        
        if (env.EnvironmentName = "Development" || env.EnvironmentName = "staging") then        
            app.UseDatabaseErrorPage() |> ignore
        

        let dbContext = app.ApplicationServices.GetService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        app.UseStaticFiles()
            .UseAuthentication()      
            .UseAuthorization()
             |>ignore

        if env.EnvironmentName = "Development" then
            app.UseDeveloperExceptionPage() |> ignore
            app.UseGiraffe Server.App.webApp
        else 
            app.UseGiraffeErrorHandler(Server.ErrorHandling.errorHandler).UseGiraffe Server.App.webApp
    



let configureAppsettings (ctx: HostBuilderContext) (config: IConfigurationBuilder) =
    config.SetBasePath(ctx.HostingEnvironment.ContentRootPath).AddJsonFile("appsettings.json")
          .AddJsonFile(sprintf "appsettings.%s.json" ctx.HostingEnvironment.EnvironmentName, true, true)
          .AddEnvironmentVariables()
    |> ignore

let configureLogging (ctx: HostBuilderContext) (logging: ILoggingBuilder) =
    logging.AddConfiguration(ctx.Configuration.GetSection("Logging")).AddConsole().AddDebug().AddSlack()
    |> ignore



[<EntryPoint>]
let main _ =
        HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureAppsettings)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseIISIntegration()
                        .Configure(configureApp)
                        |> ignore
                )
            .Build()
            .Run()
        
        0
