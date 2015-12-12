using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.General;
using MyTeam.Services.Repositories;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    class ArticleService : IArticleService
    {

        private readonly IRepository<Article> _articleRepository;
        private readonly ApplicationDbContext _dbContext;

        public ArticleService(IRepository<Article> articleRepository, ApplicationDbContext dbContext)
        {
            _articleRepository = articleRepository;
            _dbContext = dbContext;
        }

        public PagedList<ArticleViewModel> Get(Guid clubId, int skip, int take)
        {
            var query = _articleRepository.Get();
            query = query
                .Where(a => a.ClubId == clubId)
                .OrderByDescending(a => a.Published);

            var totalCount = query.Count();

            var articles = query.Select(a =>
                new ArticleViewModel
                {
                    Id = a.Id,
                    Author = new MemberViewModel {Fullname = a.Author.Fullname},
                    AuthorId = a.AuthorId,
                    Headline = a.Headline,
                    Content = a.Content,
                    GameId = a.GameId,
                    ImageUrl = a.ImageUrl,
                    Published = a.Published
                });

            return new PagedList<ArticleViewModel>(articles.Skip(skip).Take(take), skip, take, totalCount);
        }

        public ArticleViewModel Get(Guid articleId)
        {
            return _articleRepository.Get().Where(a => a.Id == articleId).Select(a =>
                new ArticleViewModel
                {
                    Id = articleId,
                    Author = new MemberViewModel { Fullname = a.Author.Fullname },
                    AuthorId = a.AuthorId,
                    Headline = a.Headline,
                    Content = a.Content,
                    GameId = a.GameId,
                    ImageUrl = a.ImageUrl,
                    Published = a.Published
                }).Single(); 
        }

        public PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take)
        {
            var skip = 0;
            var query = _articleRepository.Get()
                    .Where(a => a.ClubId == clubId)
                    .OrderByDescending(a => a.Published);

            var totalCount = query.Count();
                  
                    
              return new PagedList<SimpleArticleDto>(query.Skip(skip).Take(take)
                    .Take(take)
                    .Select(a => new SimpleArticleDto
                    {
                        Id = a.Id,
                        Headline = a.Headline,
                        Published = a.Published
                    }),
                    skip, take, totalCount);
        }



        public ArticleViewModel CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId)
        {
            var articleId = model.IsNew ? Guid.NewGuid() : model.ArticleId;

            if (model.IsNew)
            {
                var newArticle = new Article
                {
                    Id = articleId,
                    Headline = model.Headline,
                    ClubId = clubId,
                    AuthorId = authorId,
                    GameId = model.GameId,
                    Content = model.Content,
                    ImageUrl = model.ImageUrl,
                    Published = DateTime.Now
                };
                _dbContext.Add(newArticle);
                _dbContext.SaveChanges();

            }
            var article = _articleRepository.GetSingle(articleId);

            if (!model.IsNew)
            {
                article.Content = model.Content;
                article.ImageUrl = model.ImageUrl;
                article.Headline = model.Headline;
                _articleRepository.CommitChanges();
            }


            return Get(articleId); ;
        }

        public void Delete(Guid articleId)
        {
            var article = _articleRepository.GetSingle(articleId);
            _dbContext.Articles.Remove(article);
            _dbContext.SaveChanges();
        }
    }

}