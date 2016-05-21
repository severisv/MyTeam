﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyTeam.Settings;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class CloudinaryScriptsViewComponent : ViewComponent
    {
        private readonly CloudinaryOptions _configuration;

        public CloudinaryScriptsViewComponent(IOptions<CloudinaryOptions> configuration)
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