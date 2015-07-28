using System;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Events;


namespace MyTeam.Controllers
{
    public class BaseController : Controller
    {
        [FromServices]
        public IMemoryStore MemoryStore { get; set; }

        public Player ActivePlayer => MemoryStore.GetPlayerFromUser(User.Identity.Name);
        public bool CurrentUserHasPlayer => ActivePlayer != null;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.CurrentUserHasPlayer = CurrentUserHasPlayer;
        }
    }
}
