using System;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Enums;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.News;

namespace MyTeam.Controllers
{
    public class NewsController : BaseController
    {
        private const string BaseRoute = "nyheter/";

        [FromServices]
        public IArticleService ArticleService { get; set; }
        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        [Route("")]
        [Route("nyheter")]
        public IActionResult Index(int skip = 0, int take = 4)
        {
            var model = ArticleService.Get(HttpContext.GetClub().Id, skip, take);
            ViewBag.MetaDescription = DbContext.Clubs.Where(c => c.Id == Club.Id).Select(c => c.Description).Single();
            return View("Index", model);
        }

        [Route(BaseRoute+"vis")]
        public IActionResult Show(Guid articleId)
        {
            var model = ArticleService.Get(articleId);
            return View("Show", model);
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
        public IActionResult Edit(Guid articleId)
        {
            var article = ArticleService.Get(articleId);
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
                ArticleService.CreateOrUpdate(model, HttpContext.GetClub().Id, HttpContext.Member().Id);
                return RedirectToAction("Show", new {articleId = model.ArticleId});
            }
            return View("Edit", model);

        }

        [Route(BaseRoute+"slett")]
        [RequireMember(Roles.Coach, Roles.Admin, Roles.NewsWriter)]
        public IActionResult Delete(Guid articleId)
        {
            if (ModelState.IsValid)
            {
                ArticleService.Delete(articleId);

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
                var comment = ArticleService.PostComment(model.ArticleId, model.Content, CurrentMember.Id);
                return PartialView("_GetComment", comment);
            }
            return Content("");
        }



        public PartialViewResult GetComments(Guid articleId)
        {
            var comments = ArticleService.GetComments(articleId);
            return PartialView("_GetComments", new GetCommentsViewModel(comments, articleId));
        }

    }
}