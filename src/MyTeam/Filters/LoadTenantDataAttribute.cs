using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Models.Dto;
using MyTeam.Services.Application;

namespace MyTeam
{


    public static class CtxExtensions {



        public static CurrentClub GetClub(this HttpContext context) => new CurrentClub(context.Items[LoadTenantDataAttribute.ClubKey] as ClubDto);

        public static UserMember Member(this HttpContext context) => new UserMember(context.Items[LoadTenantDataAttribute.MemberKey] as PlayerDto);
    }

    public class LoadTenantDataAttribute : ActionFilterAttribute
    {
     
        public const string ClubKey = "club";
        public const string MemberKey = "member";

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
                var context = actionContext.HttpContext;
                var cacheHelper = context.RequestServices.GetService<ICacheHelper>();
                
                var clubId = GetSubdomain(context);
                if (clubId == null || clubId == "breddefotball" || clubId == "bfstaging") clubId = "wamkam";

                if (clubId != null)
                {
                    var club = cacheHelper.GetCurrentClub(clubId);
                    context.Items[ClubKey] = club;
                    var username = context.User.Identity.Name;
                    context.Items[MemberKey] = cacheHelper.GetPlayerFromUser(username, club.Id);
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