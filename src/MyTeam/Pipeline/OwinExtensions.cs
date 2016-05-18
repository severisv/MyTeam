using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Pipeline;
using MyTeam.Services.Application;

namespace MyTeam
{
    public static class OwinExtensions
    {
     
        public static void LoadTenantData(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var cacheHelper = app.ApplicationServices.GetService<ICacheHelper>();

                var clubId = GetSubdomain(context);
                if (clubId == null || clubId == "breddefotball" || clubId == "bfstaging") clubId = "wamkam";

                if (clubId != null)
                {
                    context.Items[PipelineConstants.ClubKey] = cacheHelper.GetCurrentClub(clubId);
                    var username = context.User.Identity.Name;
                    context.Items[PipelineConstants.MemberKey] = cacheHelper.GetPlayerFromUser(username, clubId);
                }
                await next();
            });
        }

        private static string GetSubdomain(HttpContext context)
        {
            var hostNameArray = context.Request.Host.Value.Split('.');
            if (hostNameArray.Length > 2)
            {
                var subdomain = hostNameArray[0];
                if ("www".Equals(subdomain, StringComparison.CurrentCultureIgnoreCase))
                    subdomain = hostNameArray[1];
                return subdomain;
            }
            return null;
        }


        public static void LogStart(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Items["RequestStart"] = DateTime.Now;
                await next();
            });
        }
    }
}