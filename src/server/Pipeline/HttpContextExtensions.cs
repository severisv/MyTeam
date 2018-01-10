using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyTeam.Models.Dto;
using MyTeam.Settings;

namespace MyTeam
{
    public static class HttpContextExtensions
    {
  
      
        public static ICloudinary Cloudinary(this HttpContext context) => context.RequestServices.GetService<ICloudinary>();
        
        public static bool UserIsMember(this HttpContext context) => context.Member().Exists;
    
        public static bool UserIsInRole(this HttpContext context, params string[] roles) => context.Member().Roles.ContainsAny(roles);
      

        public static string Elapsed(this HttpContext context)
        {
            var start = context.Items["RequestStart"] as DateTime?;
            if (start != null)
                return Math.Round((DateTime.Now - start.Value).TotalMilliseconds).ToString();
            return string.Empty;
        }
    }
}