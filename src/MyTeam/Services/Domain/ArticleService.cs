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
                    .Select(a => new SimpleArticleDto {
                        Id = a.Id,
                        Headline = a.Headline,
                        Published = a.Published
                    });
        }

  

        public Article CreateOrUpdate(EditArticleViewModel model, string clubId, Guid authorId)
        {
            var article = model.IsNew
                ? new Article
                {

                }
                : _articleRepository.GetSingle(model.ArticleId);
            return article;
        }
    }
    
}