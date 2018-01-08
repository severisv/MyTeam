using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Services.Application;

namespace MyTeam
{
    public static class AppBuilderExtensions
    {
        public static void WriteException(this IApplicationBuilder app, Exception ex)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(ex.Message);
                var exceptions = new List<Exception>();
                while (ex.InnerException != null)
                {
                    exceptions.Add(ex);
                    ex = ex.InnerException;
                }

                exceptions.Reverse();
                foreach (var exception in exceptions)
                {
                    await context.Response.WriteAsync(exception.Message);
                }
                await context.Response.WriteAsync(ex.StackTrace.ToString());
                
            });
        }

      
        public static void Write(this IApplicationBuilder app, string msg)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(msg);
            });
        }
    }
}