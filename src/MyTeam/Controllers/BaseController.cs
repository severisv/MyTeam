using System;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;


namespace MyTeam.Controllers
{
    [HandleError]
    //[LoadClub]
    public class BaseController : Controller
    {
        [FromServices]
        public IMemoryStore MemoryStore { get; set; }

        public virtual Player CurrentPlayer => MemoryStore.GetPlayerFromUser(User.Identity.Name);

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
