using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using MyTeam;

namespace Microsoft.AspNet.Mvc
{
    public class InvalidInputResult : ViewResult
    {

        public InvalidInputResult(HttpContext httpContext, ModelStateDictionary modelState)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "_InvalidInput.cshtml";
            }
            else
            {
                this.ViewName = "InvalidInput";
            }

            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = modelState
            };
        }
    }
}