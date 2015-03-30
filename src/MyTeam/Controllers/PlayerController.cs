using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.Player;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MyTeam.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IRepository<Player> _playerRepository;

        public PlayerController(IRepository<Player> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public IActionResult Index()
        {
            var model = new ShowPlayersViewModel();
            return View(model);
        }

    
    }
}
