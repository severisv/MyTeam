using System;
using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace MyTeam
{
    public static class AppBuilderExtensions
    {
        public static void WriteException(this IApplicationBuilder app, Exception ex, IHostingEnvironment env)
        {
            app.Run(async context =>
            {
                if (env.IsDevelopment() || context.User.IsDeveloper())
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
                }
            });
        }
    }
}