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
        ArticleViewModel GetMatchReport(Guid gameId);
    }
}