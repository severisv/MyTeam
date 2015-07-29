using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using MyTeam;

namespace Microsoft.AspNet.Mvc
{
    public class UnauthorizedResult : ViewResult
    {

        public UnauthorizedResult(HttpContext httpContext)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "~/Views/Shared/Error/_Unauthorized.cshtml";
            }
            else
            {
                this.ViewName = "~/Views/Shared/Error/Unauthorized.cshtml";
            }

            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = null
            };
        }
    }
}