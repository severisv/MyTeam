using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

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
    }
}