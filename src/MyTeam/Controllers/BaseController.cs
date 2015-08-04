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
        public IMemoryStore MemoryStore { get; set; }

        public virtual PlayerDto CurrentPlayer => _currentPlayer ?? (_currentPlayer = MemoryStore.GetPlayerFromUser(User.Identity.Name));
        private PlayerDto _currentPlayer;
        public virtual ClubDto Club => _club ?? (_club = MemoryStore.GetCurrentClub(RouteData.Values["club"] as string));
        private ClubDto _club;

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
