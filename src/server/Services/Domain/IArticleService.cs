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
        ArticleViewModel Get(Guid clubId, string name);
        PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take);
        string CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId);
        void Delete(Guid articleId);

        IEnumerable<SimpleGame> GetGames(DateTime now);
        ArticleViewModel GetMatchReport(Guid gameId);
    }
}