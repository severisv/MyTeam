using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace MyTeam.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return Error();
        }

        
        public IActionResult Error(Exception e = null)
        {
            if(Request.IsAjaxRequest())
                return View("~/Views/Shared/Error/_Error.cshtml", e);

            return View("~/Views/Shared/Error/Error.cshtml", e);

        }
    }
}