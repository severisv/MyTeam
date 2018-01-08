using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Attendance;

namespace MyTeam.Controllers
{
    [Route("om")]
    public class AboutController : BaseController
    {

        private readonly IAboutService _aboutService;

        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }


        [Route("")]
        public IActionResult Index()
        {
         
            return View("Index");
        }
        

    }

    
}