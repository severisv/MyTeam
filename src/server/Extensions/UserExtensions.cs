using System.Security.Principal;

namespace MyTeam
{
    public static class UserExtensions
    {
        public static bool IsDeveloper(this IPrincipal user)
        {
            return user.Identity.Name == "severin@sverdvik.no";
        }
    }
}