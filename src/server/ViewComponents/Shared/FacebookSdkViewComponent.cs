using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyTeam.Settings;


namespace MyTeam.ViewComponents.Shared
{
    public class FacebookSdkViewComponent : ViewComponent
    {
        private readonly FacebookOpts _configuration;

        public FacebookSdkViewComponent(IOptions<FacebookOpts> configuration)
        {
            _configuration = configuration.Value;
        }

        public IViewComponentResult Invoke()
        {
            return View("FacebookSdk", _configuration);
        }


    }
}