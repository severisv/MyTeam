using System;
using System.Collections.Generic;
using MyTeam.Models.Dto;
using MyTeam.Models.General;
using MyTeam.ViewModels.Game;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    public interface IArticleService
    {
        PagedList<ArticleViewModel> Get(Guid clubId, int skip, int take);
        ArticleViewModel Get(Guid clubId, string name);
        PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take);
        string CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId);
        void Delete(Guid articleId);

        IEnumerable<CommentViewModel> GetComments(Guid articleId);
        CommentViewModel PostComment(Guid articleId, string content, Guid memberId, string facebookId, string name, string userName);
        IEnumerable<SimpleGame> GetGames(DateTime now);
        ArticleViewModel GetMatchReport(Guid gameId);
    }
}