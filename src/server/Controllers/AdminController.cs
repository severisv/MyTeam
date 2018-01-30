using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Admin;

namespace MyTeam.Controllers
{
    [Route("admin")]
    [RequireMember(Roles.Coach, Roles.Admin)]
    public class AdminController : BaseController
    {
        private readonly IPlayerService _playerService;

        public AdminController(IPlayerService playerService)
        {
            _playerService = playerService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Route("spillerinvitasjon")]
        public IActionResult AddPlayers()
        {
            return View();
        }

    }
}