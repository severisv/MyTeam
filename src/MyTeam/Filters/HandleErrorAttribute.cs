using System.Collections.Generic;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Mvc;
using MyTeam.Controllers;

namespace MyTeam.Filters
{
    public class HandleErrorAttribute : ExceptionFilterAttribute
    {
       
        public override void OnException(ExceptionContext context)
        {
            context.Result = new ErrorResult(context.HttpContext, context.Exception);
        }
       
    }
    
}
