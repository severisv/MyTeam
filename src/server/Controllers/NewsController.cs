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
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NewsController(IArticleService articleService, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _articleService = articleService;
            _dbContext = dbContext;
            _userManager = userManager;
        }



        [Route(BaseRoute + "kamprapport/{gameId}")]
        public IActionResult MatchReport(Guid gameId)
        {
            var model = _articleService.GetMatchReport(gameId);
            ViewBag.IsMatchReport = true;
            return PartialView("MatchReport", model);
        }


        [Route(BaseRoute + "ny")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Create()
        {
            var model = new EditArticleViewModel
            {
                Games = _articleService.GetGames(DateTime.Now)
            };
            return View("Edit", model);
        }

        [Route(BaseRoute + "endre")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Edit(string navn)
        {
            var article = _articleService.Get(Club.Id, navn);
            var games = _articleService.GetGames(article.Published);
            var model = new EditArticleViewModel(article, games);
            return View(model);
        }

        [HttpPost]
        [Route(BaseRoute + "endre")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Edit(EditArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var name = _articleService.CreateOrUpdate(model, HttpContext.GetClub().Id, HttpContext.Member().Id);
                return RedirectToAction("Show", new { name = name });
            }
            model.Games = _articleService.GetGames(model.Published ?? DateTime.Now);
            return View("Edit", model);

        }

        [Route(BaseRoute + "slett")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Delete(Guid articleId)
        {
            if (ModelState.IsValid)
            {
                _articleService.Delete(articleId);

                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", articleId);
        }

        [Route(BaseRoute + "kommenter")]
        [HttpPost]
        [Authorize]
        public IActionResult PostComment(PostCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var facebookId = GetFacebookId();
                var comment = _articleService.PostComment(model.ArticleId, model.Content, CurrentMember.Id, facebookId, model.Name, User.Identity.Name);
                return PartialView("_GetComment", comment);
            }
            return Content("");
        }

        public PartialViewResult GetComments(Guid articleId)
        {
            var comments = _articleService.GetComments(articleId);

            var facebookId = GetFacebookId();

            return PartialView("_GetComments", new GetCommentsViewModel(comments, articleId, facebookId));
        }

        private string GetFacebookId()
        {
            string facebookId = null;
            if (User.Identity.IsAuthenticated && !CurrentMember.Exists)
            {
                var user = _userManager.FindByEmailAsync(User.Identity.Name).Result;
                facebookId =
                    _dbContext.UserLogins.Where(u => u.LoginProvider == "Facebook" && u.UserId == user.Id)
                        .Select(u => u.ProviderKey)
                        .FirstOrDefault();
            }
            return facebookId;
        }
    }
}
