using Microsoft.AspNet.Mvc;

namespace MyTeam.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}