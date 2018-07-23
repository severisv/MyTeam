using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.News;
using NotFoundResult = MyTeam.Extensions.Mvc.NotFoundResult;


namespace MyTeam.Controllers
{
    public class NewsController : BaseController
    {
        private const string BaseRoute = "nyheter/";


        private readonly IArticleService _articleService;


        public NewsController(IArticleService articleService)
        {
            _articleService = articleService;
        }



        [Route(BaseRoute + "kamprapport/{gameId}")]
        public IActionResult MatchReport(Guid gameId)
        {
            var model = _articleService.GetMatchReport(gameId);
            ViewBag.IsMatchReport = true;
            return PartialView("MatchReport", model);
        }

    }

}
