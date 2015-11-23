using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    class ArticleService : IArticleService
    {

        private readonly IRepository<Article> _articleRepository;

        public ArticleService(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public IEnumerable<Article> Get(string clubId, int skip, int take)
        {
            return
                _articleRepository.Get()
                    .Where(a => a.ClubId == clubId)
                    .OrderByDescending(a => a.Published)
                    .Skip(skip)
                    .Take(take);
        }

        public Article Get(Guid articleId)
        {
            return _articleRepository.GetSingle(articleId);
        }

        public IEnumerable<SimpleArticleDto> GetSimple(string clubId, int take)
        {
            return _articleRepository.Get()
                    .Where(a => a.ClubId == clubId)
                    .OrderByDescending(a => a.Published)
                    .Take(take)
                    .Select(a => new SimpleArticleDto
                    {
                        Id = a.Id,
                        Headline = a.Headline,
                        Published = a.Published
                    });
        }



        public Article CreateOrUpdate(EditArticleViewModel model, string clubId, Guid authorId)
        {
            var articleId = model.IsNew ? Guid.NewGuid() : model.ArticleId;

            if (model.IsNew)
            {
                var newArticle = new Article
                {
                    Id = articleId,
                    ClubId = clubId,
                    AuthorId = authorId,
                    GameId = model.GameId,
                    Content = model.Content,
                    ImageUrl = model.ImageUrl,
                    Published = DateTime.Now
                };
                _articleRepository.Add(newArticle);
                _articleRepository.CommitChanges();

            }
            var article = _articleRepository.GetSingle(articleId);

            if (!model.IsNew)
            {
                article.Content = model.Content;
                article.ImageUrl = model.ImageUrl;
                article.Headline = model.Headline;
                _articleRepository.CommitChanges();
            }
            return article;
        }

        public void Delete(Guid articleId)
        {
            var article = _articleRepository.GetSingle(articleId);
            _articleRepository.Delete(article);
            _articleRepository.CommitChanges();
        }
    }

}