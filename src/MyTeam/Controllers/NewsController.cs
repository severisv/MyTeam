using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.News;

namespace MyTeam.Controllers
{
    public class NewsController : BaseController
    {
        private const string BaseRoute = "nyheter/";

     
        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _dbContext;

        public NewsController(IArticleService articleService, ApplicationDbContext dbContext)
        {
            _articleService = articleService;
            _dbContext = dbContext;
        }


        [Route("{skip:int?}/{take:int?}")]
        [Route("nyheter/{skip:int?}/{take:int?}")]
        public IActionResult Index(int skip = 0, int take = 4)
        {
            var model = _articleService.Get(HttpContext.GetClub().Id, skip, take);
            ViewBag.MetaDescription = _dbContext.Clubs.Where(c => c.Id == Club.Id).Select(c => c.Description).Single();
            return View("Index", model);
        }

        [Route(BaseRoute+"vis/{name}")]
        public IActionResult Show(string name)
        {
            var model = _articleService.Get(Club.Id, name);
            return View("Show", model);
        }

        [Route(BaseRoute + "vis")]
        public IActionResult ShowFallback(Guid articleId)
        {
            var article = _dbContext.Articles.Single(a => a.Id == articleId);
            return RedirectToAction("Show", new {name = article.Name});
        }


        [Route(BaseRoute+"ny")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Create()
        {
            var model = new EditArticleViewModel();
            return View("Edit", model);
        }

        [Route(BaseRoute+"endre")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Edit(string navn)
        {
            var article = _articleService.Get(Club.Id, navn);
            var model = new EditArticleViewModel(article);
            return View(model);
        }

        [HttpPost]
        [Route(BaseRoute+"endre")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Edit(EditArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var name = _articleService.CreateOrUpdate(model, HttpContext.GetClub().Id, HttpContext.Member().Id);
                return RedirectToAction("Show", new { name = name});
            }
            return View("Edit", model);

        }

        [Route(BaseRoute+"slett")]
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
        [RequireMember]
        [HttpPost]
        public IActionResult PostComment(PostCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = _articleService.PostComment(model.ArticleId, model.Content, CurrentMember.Id);
                return PartialView("_GetComment", comment);
            }
            return Content("");
        }
        
        public PartialViewResult GetComments(Guid articleId)
        {
            var comments = _articleService.GetComments(articleId);
            return PartialView("_GetComments", new GetCommentsViewModel(comments, articleId));
        }

    }
}