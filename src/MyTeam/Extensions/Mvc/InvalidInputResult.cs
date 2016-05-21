using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyTeam;

namespace Microsoft.AspNetCore.Mvc
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