using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyTeam;

namespace Microsoft.AspNetCore.Mvc
{
    public class UnauthorizedResult : ViewResult
    {

        public UnauthorizedResult(HttpContext httpContext)
        {
            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = null
            };


            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "_Unauthorized";
            }
            else if (httpContext.User.Identity.IsAuthenticated)
            {
                this.ViewName = "Unauthorized";
            }
            else
            {
                this.ViewName = "Login";
                this.ViewData.Add("returnUrl", httpContext.Request.Path.Value);
            }

           
        }
    }
}