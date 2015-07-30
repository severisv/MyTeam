using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace MyTeam.Controllers
{
    public class ErrorController : BaseController
    {
        public IActionResult Index()
        {
            return Error();
        }

        
        public IActionResult Error(Exception e = null)
        {
            if(Request.IsAjaxRequest())
                return View("_Error", e);

            return View("Error", e);

        }
    }
}