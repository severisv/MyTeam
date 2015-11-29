using Microsoft.AspNet.Http;
using MyTeam.Models.Dto;
using MyTeam.Pipeline;

namespace MyTeam
{
    public static class HttpContextExtensions
    {
  

        public static CurrentClub GetClub(this HttpContext context)
        {
            return new CurrentClub(context.Items[PipelineConstants.ClubKey] as ClubDto);
        }

        public static UserMember Member(this HttpContext context)
        {
            return new UserMember(context.Items[PipelineConstants.MemberKey] as PlayerDto);
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