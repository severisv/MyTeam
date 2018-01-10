﻿using System;
using Microsoft.AspNetCore.Mvc;
using MyTeam.Services.Domain;

namespace MyTeam.ViewComponents.News
{
    public class ArticleNavViewComponent : ViewComponent
    {
        private readonly IArticleService _articleService;

        public ArticleNavViewComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IViewComponentResult Invoke(Guid clubId)
        {
            var articles = _articleService.GetSimple(clubId, 8);
            return View("_ArticleNav", articles);
        }

        
    }
}