using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyTeam.Services.Application;

namespace MyTeam.Filters
{
    public class HandleErrorAttribute : ExceptionFilterAttribute
    {
       
        public override void OnException(ExceptionContext context)
        {
            var env = context.HttpContext.RequestServices.GetService<IHostingEnvironment>();

            if (!env.IsDevelopment())
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<HandleErrorAttribute>>();
                var slack = context.HttpContext.RequestServices.GetService<SlackService>();
                var request = context.HttpContext.Request;
                slack.Log(context.Exception, $"{request.Method}: {request.Path}{request.QueryString}");
                var eventId = (DateTime.Now - DateTime.Today).TotalSeconds;
                logger.LogError((int)eventId, $"Error in {context.HttpContext.Request.Path}", context.Exception);
                context.Result = new ErrorResult(context.HttpContext, context.Exception);
            }
        }
    }
    
}
