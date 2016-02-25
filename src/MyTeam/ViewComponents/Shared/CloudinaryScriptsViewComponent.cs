using System;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class CloudinaryScriptsViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public CloudinaryScriptsViewComponent(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CloudinaryConfiguration(_configuration);

            return View("_CloudinaryScripts", model);
        }


    }
}