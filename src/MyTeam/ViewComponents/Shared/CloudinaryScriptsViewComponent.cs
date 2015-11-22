using System;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class CloudinaryScriptsViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public CloudinaryScriptsViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            return View("_CloudinaryScripts", _configuration);
        }


    }
}