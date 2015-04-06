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
        private readonly IRepository<Player> _playerRepository;

        public PlayerController(IRepository<Player> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public IActionResult Index(PlayerStatus type = PlayerStatus.Aktiv)
        {
            var players = _playerRepository.Get().Where(p => p.Status == type);
            ViewBag.PageName = Res.PlayersOfType(type);

            var model = new ShowPlayersViewModel(players);
            return View(model);
        }
    

        public IActionResult Show(Guid playerId)
        {
            var player = _playerRepository.GetSingle(playerId);
            return View("_Show", player);
        }

    
    }
}
