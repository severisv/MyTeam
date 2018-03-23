using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyTeam;

namespace Microsoft.AspNet.Mvc
{
    public class ErrorResult : ViewResult
    {
    
        public ErrorResult(HttpContext httpContext, Exception exception = null)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                this.ViewName = "_Error";
            }
            else
            {
                this.ViewName = "Error";
                StatusCode = 500;
            }

            this.ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = exception
            };
        }
    }
}

