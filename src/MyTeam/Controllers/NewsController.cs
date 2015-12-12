﻿using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Mvc;
using MyTeam.Filters;
using MyTeam.Models.Enums;
using MyTeam.Resources;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.News;
using MyTeam.ViewModels.Table;

namespace MyTeam.Controllers
{
    public class NewsController : BaseController
    {
        private const string BaseRoute = "nyheter/";

        [FromServices]
        public IArticleService ArticleService { get; set; }

        [Route("")]
        [Route("nyheter")]
        public IActionResult Index(int skip = 0, int take = 4)
        {
            var model = ArticleService.Get(HttpContext.GetClub().Id, skip, take);
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
                var article = ArticleService.CreateOrUpdate(model, HttpContext.GetClub().Id, HttpContext.Member().Id);
                return View("Show", article);
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



    }
}