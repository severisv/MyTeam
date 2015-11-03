using Microsoft.AspNet.Mvc;
using MyTeam.Controllers;

namespace MyTeam.Filters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
       

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid){
                actionContext.Result = new InvalidInputResult(actionContext.HttpContext, actionContext.ModelState);
            }
        }
    }
}
