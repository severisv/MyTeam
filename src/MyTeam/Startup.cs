﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Authentication.Cookies;
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
using MyTeam.Services;
using MyTeam.Services.Composition;
using MyTeam.Services.Repositories;


namespace MyTeam
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

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

                options.Cookies.ApplicationCookie.CookieName = "_myt";
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(8);
                options.Cookies.ApplicationCookie.SlidingExpiration = true;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequiredLength = 7;
            }))
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddInstance(Configuration);

            Registrations.Setup(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.LogStart();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                    app.UseBrowserLink();
                }
                else
                {
                    app.UseExceptionHandler("/Error/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                        //                            dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                        dbContext.Database.Migrate();
                        BootstrapData.Initialize(app.ApplicationServices);
                    }
                }
                catch
                {
                }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("nb-NO") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("nb-NO") }
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



        //catch (Exception ex)
        //{
        //    app.Run(async context =>
        //    {
        //        context.Response.ContentType = "text/plain";
        //        await context.Response.WriteAsync(ex.Message);
        //        var exceptions = new List<Exception>();
        //        while (ex.InnerException != null)
        //        {
        //            exceptions.Add(ex);
        //            ex = ex.InnerException;
        //        }

        //        exceptions.Reverse();
        //        foreach (var exception in exceptions)
        //        {
        //            await context.Response.WriteAsync(exception.Message);
        //        }

        //        await context.Response.WriteAsync(ex.StackTrace.ToString());


        //    });
        //}

        //}
        //      }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
