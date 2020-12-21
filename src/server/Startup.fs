module MyTeam.Startup

open System;
open Microsoft.AspNetCore.Builder;
open Microsoft.AspNetCore.Hosting;
open Microsoft.EntityFrameworkCore;
open Microsoft.Extensions.Configuration;
open Microsoft.Extensions.DependencyInjection;
open MyTeam.Models;
open Microsoft.AspNetCore.Identity;
open Microsoft.AspNetCore.Authentication.Cookies;
open Services.Utils;

let configureServices (services: IServiceCollection) =
        let configuration = services.BuildServiceProvider().GetService<IConfiguration>()
        services.AddDbContext<ApplicationDbContext>(fun options ->
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")) |> ignore) |> ignore

        services
            .AddMemoryCache()
            .AddAntiforgery()
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
        
        Server.App.addGiraffe(services);
        Server.App.registerJsonSerializers(services);
        services.AddTransient<EmailSender>() |> ignore
        
        


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

        Server.App.useGiraffe(app);
    

