using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class NewsController : BaseController
    {
        [FromServices]
        public IArticleService ArticleService { get; set; }


        public IActionResult Index(int skip = 0, int take = 5)
        {
            var model = ArticleService.Get(Context.GetClub().ClubId, skip, take);
            return View("Index", model);
        }

        public IActionResult Show(Guid articleId)
        {
            var model = ArticleService.Get(articleId);
            return View("Show ", model);
        }



        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(Guid seasonId)
        {
       
            return View();
        }

        [HttpPost]
        [RequireMember(Roles.Coach, Roles.Admin)]
        public IActionResult Update(CreateTableViewModel model)
        {
       
            return View(model);

        }
    


    }
}