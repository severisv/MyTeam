using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MyTeam.Models;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.General;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext _dbContext;

        public ArticleService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        private static IQueryable<ArticleViewModel> Select(IQueryable<Article> query)
        {
            var result = query
                .Select(a =>
                    new ArticleViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Author = new MemberViewModel { Fullname = a.Author.Fullname, UrlName = a.Author.UrlName },
                        AuthorId = a.AuthorId,
                        Headline = a.Headline,
                        Content = a.Content,
                        ImageUrl = a.ImageUrl,
                        Published = a.Published,
                        GameId = a.GameId,
                        CommentCount = (int)a.Comments.Count()
                    });
            return result;
        }


        public ArticleViewModel GetMatchReport(Guid gameId)
        {
            var query = _dbContext.Articles.Where(a => a.GameId == gameId);
            var result = Select(query);
            return result.FirstOrDefault();
        }
    }

}