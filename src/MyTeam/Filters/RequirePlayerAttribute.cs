using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Controllers;

namespace MyTeam.Filters
{
    public class RequirePlayerAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var controller = (BaseController)actionContext.Controller;

            if (!controller.UserIsPlayer())
            {
                actionContext.Result = new UnauthorizedResult(actionContext.HttpContext);
            }
        }
    }
}