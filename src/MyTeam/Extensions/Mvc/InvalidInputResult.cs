using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using MyTeam;

namespace Microsoft.AspNet.Mvc
{
    public class InvalidInputResult : ViewResult
    {

        public InvalidInputResult(HttpContext httpContext, ModelStateDictionary modelState)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "~/Views/Shared/Error/_InvalidInput.cshtml";
            }
            else
            {
                this.ViewName = "~/Views/Shared/Error/InvalidInput.cshtml";
            }

            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = modelState
            };
        }
    }
}