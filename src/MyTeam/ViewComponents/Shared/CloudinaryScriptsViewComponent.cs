using System;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class CloudinaryScriptsViewComponent : ViewComponent
    {
        private readonly IConfigurationRoot _configuration;

        public CloudinaryScriptsViewComponent(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            return View("_CloudinaryScripts", _configuration);
        }


    }
}