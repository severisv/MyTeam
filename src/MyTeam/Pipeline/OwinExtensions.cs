using Microsoft.AspNet.Builder;
using MyTeam.Pipeline;
using MyTeam.Services.Application;

namespace MyTeam
{
    public static class OwinExtensions
    {
     
        public static void LoadTenantData(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var cacheHelper = app.ApplicationServices.GetService(typeof(ICacheHelper)) as ICacheHelper;

                var clubId = PipelineConstants.GetClubIdFromHostname(context.Request.Host.Value);

                // If there is no registered clubId for the domain, get from querystring
                if (clubId == null)
                {
                    clubId = context.Request.Path.ToString().Split('/')[1];
                }
                context.Items[PipelineConstants.ClubKey] = cacheHelper.GetCurrentClub(clubId);
                var username = context.User.Identity.Name;
                context.Items[PipelineConstants.MemberKey] = cacheHelper.GetPlayerFromUser(username, clubId);
                await next();
            });
        }
        
   }
}