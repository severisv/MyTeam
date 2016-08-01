using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Pipeline;
using MyTeam.Services.Application;

namespace MyTeam.Filters
{
    public class LoadTenantDataAttribute : ActionFilterAttribute
    {
     
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
                var context = actionContext.HttpContext;
                var cacheHelper = context.RequestServices.GetService<ICacheHelper>();
                
                var clubId = GetSubdomain(context);
                if (clubId == null || clubId == "breddefotball" || clubId == "bfstaging") clubId = "wamkam";

                if (clubId != null)
                {
                    var club = cacheHelper.GetCurrentClub(clubId);
                    context.Items[PipelineConstants.ClubKey] = club;
                    var username = context.User.Identity.Name;
                    context.Items[PipelineConstants.MemberKey] = cacheHelper.GetPlayerFromUser(username, club.Id);
                }
           
        }

        private static string GetSubdomain(HttpContext context)
        {
            var hostNameArray = context.Request.Host.Value.Split('.');
            if (hostNameArray.Length > 2)
            {
                var subdomain = hostNameArray[0];
                if ("www".Equals(subdomain, StringComparison.CurrentCultureIgnoreCase))
                    subdomain = hostNameArray[1];
                return subdomain;
            }
            return null;
        }

    }
    
}