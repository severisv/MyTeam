using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public PagedList<ArticleViewModel> Get(Guid clubId, int skip, int take)
        {
            var query = _dbContext.Articles.Where(a => a.ClubId == clubId)
                .OrderByDescending(a => a.Published);

            var totalCount = query.Count();

            var articles = query.Select(a =>
                new ArticleViewModel
                {
                    Name = a.Name,
                    Author = new MemberViewModel { Fullname = a.Author.Fullname },
                    AuthorId = a.AuthorId,
                    Headline = a.Headline,
                    Content = a.Content,
                    ImageUrl = a.ImageUrl,
                    Published = a.Published,
                    CommentCount = (int)a.Comments.Count()
                });

            return new PagedList<ArticleViewModel>(articles.Skip(skip).Take(take), skip, take, totalCount);
        }

        public ArticleViewModel Get(Guid clubId, string name)
        {
            var query = _dbContext.Articles.Where(a => a.Name == name && a.ClubId == clubId);
                
            var result = Select(query);

            return result.First();
        }

        private static IQueryable<ArticleViewModel> Select(IQueryable<Article> query)
        {
            var result = query
                .Select(a =>
                    new ArticleViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Author = new MemberViewModel {Fullname = a.Author.Fullname},
                        AuthorId = a.AuthorId,
                        Headline = a.Headline,
                        Content = a.Content,
                        ImageUrl = a.ImageUrl,
                        Published = a.Published,
                        GameId = a.GameId,
                        CommentCount = (int) a.Comments.Count()
                    });
            return result;
        }

        public PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take)
        {
            var skip = 0;
            var query = _dbContext.Articles
                    .Where(a => a.ClubId == clubId)
                    .OrderByDescending(a => a.Published);

            var totalCount = query.Count();


            return new PagedList<SimpleArticleDto>(query.Skip(skip).Take(take)
                  .Take(take)
                  .Select(a => new SimpleArticleDto
                  {
                      Id = a.Id,
                      Headline = a.Headline,
                      Published = a.Published,
                      Name = a.Name
                  }),
                  skip, take, totalCount);
        }



        public string CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId)
        {
            var articleId = model.IsNew ? Guid.NewGuid() : model.ArticleId;

            if (model.IsNew)
            {
              
                var name = CreateArticleName(model.Headline);

                var newArticle = new Article
                {
                    Id = articleId,
                    Headline = model.Headline,
                    Name = name,
                    ClubId = clubId,
                    AuthorId = authorId,
                    Content = model.Content,
                    ImageUrl = model.ImageUrl,
                    Published = DateTime.Now,
                    GameId = model.IsMatchReport ? model.GameId : null
            };
                _dbContext.Add(newArticle);
                _dbContext.SaveChanges();

            }
            var article = _dbContext.Articles.Single(a => a.Id == articleId);

            if (!model.IsNew)
            {
                article.Content = model.Content;
                article.ImageUrl = model.ImageUrl;
                article.Headline = model.Headline;
                article.GameId = model.IsMatchReport ? model.GameId : null;
                _dbContext.SaveChanges();
            }
            return article.Name;
        }

        private string CreateArticleName(string headline)
        {
            var name = headline.Replace(" ", "-").ToLower();
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            name = rgx.Replace(name, "");

            var i = 1;
            while (true)
            {
                var existingArticles = _dbContext.Articles.Count(a => a.Name == name);
                if (existingArticles > 0) name += $"-{i}";
                else break;
                i++;
            }
            return name;
        }

        public void Delete(Guid articleId)
        {
            var article = _dbContext.Articles.Single(a => a.Id == articleId);
            _dbContext.Articles.Remove(article);
            _dbContext.SaveChanges();
        }

        public IEnumerable<CommentViewModel> GetComments(Guid articleId)
        {
            var query = _dbContext.Comments.Where(c => c.ArticleId == articleId);
            return query.Select(a =>
                new CommentViewModel(
                    new CommentMemberViewModel(a.Member.Fullname, a.Member.ImageFull, a.MemberId, a.Member.FacebookId),
                    articleId, a.Date, a.Content)).ToList().OrderBy(c => c.Date);
        }

        public CommentViewModel PostComment(Guid articleId, string content, Guid memberId)
        {


            var comment = new Comment
            {
                ArticleId = articleId,
                Content = content,
                MemberId = memberId,
                Date = DateTime.Now,
            };

            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();

            var member =_dbContext.Members.Where(m => m.Id == memberId)
                    .Select(m => new CommentMemberViewModel(m.Fullname, m.ImageFull, m.Id, m.FacebookId))
                    .Single();
            
            return new CommentViewModel(member, articleId, comment.Date, content);
        }

        public IEnumerable<SimpleGame> GetGames(DateTime date)
            =>
                _dbContext.Games.Where(g => g.DateTime <= date && g.DateTime >= date.AddDays(-14))
                    .OrderByDescending(g => g.DateTime)
                    .Select(g => new SimpleGame(g.Id, g.Team.ShortName, g.Opponent)).ToList();

        public ArticleViewModel GetMatchReport(Guid gameId)
        {
            var query = _dbContext.Articles.Where(a => a.GameId == gameId);
            var result = Select(query);
            return result.FirstOrDefault();
        }
    }
    
}