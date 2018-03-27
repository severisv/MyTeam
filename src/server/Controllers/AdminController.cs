using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;
using MyTeam.Services.Domain;

namespace MyTeam.Controllers
{
    [Route("admin")]
    [RequireMember(Roles.Coach, Roles.Admin)]
    public class AdminController : BaseController
    {

        [Route("spillerinvitasjon")]
        public IActionResult AddPlayers()
        {
            return View();
        }

    }
}