using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyTeam.Filters
{
    public class HandleErrorAttribute : ExceptionFilterAttribute
    {
       
        public override void OnException(ExceptionContext context)
        {
            var env = context.HttpContext.RequestServices.GetService<IHostingEnvironment>();

            if (!env.IsDevelopment())
            {
                var request = context.HttpContext.Request;
                var logger = context.HttpContext.RequestServices.GetService<ILogger<HandleErrorAttribute>>();
                logger.LogError(context.Exception, $"Error in {request.Method}: {request.Path}{request.QueryString} \nReferer: {request.Headers["Referer"]}");
                context.Result = new ErrorResult(context.HttpContext, context.Exception);
            }
        }
    }

}
