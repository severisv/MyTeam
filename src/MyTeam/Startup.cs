﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Localization;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyTeam.Models;
using MyTeam.Services.Composition;
using MyTeam.Settings;


namespace MyTeam
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>((options =>
            {

                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(16);
                options.Cookies.ApplicationCookie.SlidingExpiration = true;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequiredLength = 7;
            }))
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<CloudinaryOptions>(Configuration.GetSection("Integration:Cloudinary"));

            services.AddMvc();
            
            services.AddInstance(Configuration);
            services.RegisterDependencies();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            try
            {
                app.LogStart();

                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                if (env.IsDevelopment() || env.IsStaging())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error/Error");
                }

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                        dbContext.Database.Migrate();
                    }
                }
                catch (Exception e)
                {
                    if (env.IsDevelopment()) app.WriteException(e);
                }

                app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

                app.UseStaticFiles();

                app.UseIdentity();

                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    SupportedCultures = new List<CultureInfo> {new CultureInfo("nb-NO")},
                    SupportedUICultures = new List<CultureInfo> {new CultureInfo("nb-NO")}
                }, new RequestCulture(new CultureInfo("nb-NO")));

                app.UseFacebookAuthentication(options =>
                {
                    options.AppId = Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                });

                app.LoadTenantData();
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=News}/{action=Index}/{id?}");
                });


                if (env.IsProduction())
                {
                    app.Run(async context =>
                    {
                        context.Response.Redirect("/404");
                        await context.Response.WriteAsync("");
                    });
                }

          

            }
            catch (Exception e)
            {
                if (env.IsDevelopment()) app.WriteException(e);
                else app.Write("Det oppstod en feil");
            }
        }
        
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
