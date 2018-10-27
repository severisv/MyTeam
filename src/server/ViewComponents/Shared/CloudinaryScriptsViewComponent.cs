using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyTeam.Settings;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class CloudinaryScriptsViewComponent : ViewComponent
    {
        private readonly CloudinarySettings _configuration;

        public CloudinaryScriptsViewComponent(IOptions<CloudinarySettings> configuration)
        {
            _configuration = configuration.Value;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CloudinaryConfiguration(_configuration);

            return View("_CloudinaryScripts", model);
        }


    }
}