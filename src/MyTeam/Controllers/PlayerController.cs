using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Resources;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;


namespace MyTeam.Controllers
{
    public class PlayerController : Controller
    {
        [FromServices]
        public IRepository<Player> PlayerRepository { get; set; }

        
        public IActionResult Index(PlayerStatus type = PlayerStatus.Aktiv)
        {
            var players = PlayerRepository.Get().Where(p => p.Status == type);
            ViewBag.PageName = Res.PlayersOfType(type);

            var model = new ShowPlayersViewModel(players);
            return View(model);
        }
    

        public IActionResult Show(Guid playerId)
        {
            var player = PlayerRepository.GetSingle(playerId);
            return View("_Show", player);
        }

    
    }
}
