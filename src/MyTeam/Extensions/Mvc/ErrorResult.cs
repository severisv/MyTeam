using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using MyTeam;

namespace Microsoft.AspNet.Mvc
{
    public class ErrorResult : ViewResult
    {
    
        public ErrorResult(HttpContext httpContext, Exception exception = null)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "~/Views/Shared/Error/_Error.cshtml";
            }
            else
            {
                this.ViewName = "~/Views/Shared/Error/Error.cshtml";
                StatusCode = 500;
            }

            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = exception
            };
        }
    }
}

