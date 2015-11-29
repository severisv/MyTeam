using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.Models.General;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    public interface IArticleService
    {
        PagedList<Article> Get(Guid clubId, int skip, int take);
        Article Get(Guid articleId);
        PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take);
        Article CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId);
        void Delete(Guid articleId);
    }
}