using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Models.Domain;
using MyTeam.Services.Application;
using MyTeam.Services.Repositories;

namespace MyTeam.Controllers
{
    public class ClubController : BaseController
    {
     
        public IActionResult Index()
        {
            //TempData.
            return View();
        }
        
    }
}  