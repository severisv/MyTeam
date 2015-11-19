using Microsoft.AspNet.Mvc;
using MyTeam.Services.Domain;
using MyTeam.ViewModels.Shared;

namespace MyTeam.ViewComponents.News
{
    public class ArticleNavViewComponent : ViewComponent
    {
        private readonly IArticleService _articleService;

        public ArticleNavViewComponent(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IViewComponentResult Invoke(string clubId)
        {
            var articles = _articleService.GetSimple(clubId, 30);
            return View("_ArticleNav", articles);
        }

        
    }
}