using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using MyTeam;

namespace Microsoft.AspNet.Mvc
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