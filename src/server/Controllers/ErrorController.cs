﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace MyTeam.Controllers
{
    [Route("")]
    public class ErrorController : BaseController
    {

        [Route("error")]
        public IActionResult Error(Exception e = null)
        {
            if(Request.IsAjaxRequest())
                return PartialView("_Error", e);

            return View("Error", e);

        }

        [Route("404")]
        public IActionResult NotFoundAction()
        {
            HttpContext.Response.StatusCode = 404;
            if(Request.IsAjaxRequest())
                return PartialView("_NotFound");

            return View("NotFound");

        }
    }
}