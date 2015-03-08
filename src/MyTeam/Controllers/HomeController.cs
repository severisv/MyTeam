using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace MyTeam.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            ViewBag.Message = "Test";
            return View();
        }
     
        
    }
}  