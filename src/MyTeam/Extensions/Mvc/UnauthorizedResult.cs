using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using MyTeam;

namespace Microsoft.AspNet.Mvc
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
            else if (httpContext.User.IsSignedIn())
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