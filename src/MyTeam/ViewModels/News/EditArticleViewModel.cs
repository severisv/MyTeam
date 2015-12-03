using System;
using MyTeam.Filters;
using MyTeam.Models.Domain;
using MyTeam.Validation.Attributes;

namespace MyTeam.ViewModels.News
{
    public class EditArticleViewModel 
    {

        public Guid ArticleId { get; set; }
        [RequiredNO]
        public string Headline { get; set; }
        [RequiredNO]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public Guid? GameId { get; set; }
        public bool IsNew => ArticleId == Guid.Empty;

        public EditArticleViewModel()
        {
            
        }

        public EditArticleViewModel(ArticleViewModel article)
        {
            ArticleId = article.Id;
            Headline = article.Headline;
            Content = article.Content;
            ImageUrl = article.ImageUrl;
            GameId = article.GameId;
        }
    }
}
