using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyTeam;

namespace Microsoft.AspNetCore.Mvc
{
    public class NotFoundResult : ViewResult
    {

        public NotFoundResult(HttpContext httpContext)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "_NotFound";
            }
            else
            {
                this.ViewName = "NotFound";
            }
            
        }
    }
}