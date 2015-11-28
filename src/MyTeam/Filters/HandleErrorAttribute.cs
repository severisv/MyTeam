using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

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
