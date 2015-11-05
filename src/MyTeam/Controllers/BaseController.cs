using System;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;


namespace MyTeam.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        [FromServices]
        public ICacheHelper CacheHelper { get; set; }

        public virtual PlayerDto CurrentPlayer => CacheHelper.GetPlayerFromUser(User.Identity.Name);
        public virtual ClubDto Club => CacheHelper.GetCurrentClub(RouteData.Values["club"] as string);

        public bool UserIsPlayer()
        {
            return CurrentPlayer != null;
        }
        
        public void Alert(AlertType type, string message)
        {
            ViewData.Add($"Alert{type}", message);
        }
     
    }
}
