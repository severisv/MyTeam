using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using MyTeam.Models.Dto;
using MyTeam.Services.Application;

namespace MyTeam
{
    public static class OwinExtensions
    {
        private const string MemberKey = "member";
        private const string ClubKey = "club";

        public static void LoadAppData(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var cacheHelper = app.ApplicationServices.GetService(typeof(ICacheHelper)) as ICacheHelper;
                var clubId = context.Request.Path.ToString().Split('/')[1];
                context.Items[ClubKey] = cacheHelper.GetCurrentClub(clubId);
                var username = context.User.Identity.Name;
                context.Items[MemberKey] = cacheHelper.GetPlayerFromUser(username, clubId);
                await next();
            });
        }


        public static CurrentClub GetClub(this HttpContext context)
        {
            return new CurrentClub(context.Items[ClubKey] as ClubDto);
        }

        public static UserMember Member(this HttpContext context)
        {
            return new UserMember(context.Items[MemberKey] as PlayerDto);
        }

        public static bool UserIsMember(this HttpContext context)
        {
            return context.Member().Exists;
        }
        public static bool UserIsInRole(this HttpContext context, params string[] roles)
        {
            return context.Member().Roles.ContainsAny(roles);
        }
        
   }
}