using System;
using Microsoft.AspNetCore.Builder;

namespace MyTeam
{
    public static class OwinExtensions
    {
     
       
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