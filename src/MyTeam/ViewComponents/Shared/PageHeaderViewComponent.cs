using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.Shared
{
    public class PageHeaderViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(string headline, string content)
        {
            var model = new PageHeaderViewModel(headline, content);
            return View("_PageHeader", model);
        }

        public IViewComponentResult Invoke(string headline)
        {
           return Invoke(headline, "");
        }
    }
}