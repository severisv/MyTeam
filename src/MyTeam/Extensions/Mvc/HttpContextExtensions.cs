using System;
using Microsoft.AspNet.Diagnostics.Views;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using MyTeam;
using MyTeam.Models.Domain;

namespace Microsoft.AspNet.Mvc
{
    public static class HttpContextExtensions
    {
      
        public static Club GetClub(this HttpContext context)
        {
            return context.Items["club"] as Club;
        }
        
    }
}

