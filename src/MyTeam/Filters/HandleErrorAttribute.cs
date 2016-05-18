using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyTeam.Filters
{
    public class HandleErrorAttribute : ExceptionFilterAttribute
    {
       
        public override void OnException(ExceptionContext context)
        {
            var env = context.HttpContext.ApplicationServices.GetService<IHostingEnvironment>();

            if (!env.IsDevelopment())
            {
                var logger = context.HttpContext.ApplicationServices.GetService<ILogger<HandleErrorAttribute>>();
                var eventId = (DateTime.Now - DateTime.Today).TotalSeconds;
                logger.LogError((int)eventId, $"Error in {context.HttpContext.Request.Path}", context.Exception);
                context.Result = new ErrorResult(context.HttpContext, context.Exception);
            }
        }
       
    }
    
}
