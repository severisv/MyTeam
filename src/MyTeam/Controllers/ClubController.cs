using Microsoft.AspNet.Mvc;

namespace MyTeam.Controllers
{
    public class ClubController : Controller
    {
        
        public IActionResult Index()
        {
            ViewBag.Message = "Cool";
            return View();
        }
        
    }
}  