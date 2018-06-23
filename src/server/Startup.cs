using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Models;
using MyTeam.Services.Composition;
using MyTeam.Settings;
using MyTeam.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyTeam
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 7;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                    {
                        o.Cookie.Expiration = TimeSpan.FromDays(16);
                        o.Cookie.Name = "_myt";
                        o.SlidingExpiration = true;
                    }

                )
                .AddFacebook(o =>
                {
                    o.AppId = Configuration["Authentication:Facebook:AppId"];
                    o.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                });


            services.Configure<CloudinaryOptions>(Configuration.GetSection("Integration:Cloudinary"));
            services.Configure<CloudinarySettings>(Configuration.GetSection("Integration:Cloudinary"));
            services.Configure<FacebookOpts>(Configuration.GetSection("Authentication:Facebook"));
            services.Configure<FacebookOptions>(Configuration.GetSection("Authentication:Facebook"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddLocalization();
            services.AddMvc(setup => { setup.ConfigureFilters(); });
            App.addGiraffe(services);
            App.registerJsonSerializers(services);

            services.RegisterDependencies();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.LogStart();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
            }

            var dbContext = app.ApplicationServices.GetService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        
            app.UseStaticFiles();
            app.UseAuthentication();
            // app.UseRequestLocalization(new RequestLocalizationOptions
            // {
            //     DefaultRequestCulture = new RequestCulture("nb-NO"),
            //     SupportedCultures = new List<CultureInfo> { new CultureInfo("nb-NO") },
            //     SupportedUICultures = new List<CultureInfo> { new CultureInfo("nb-NO") }
            // });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=News}/{action=Index}/{id?}");
            });

            App.useGiraffe(app);

        }
    }
}
