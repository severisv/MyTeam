using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;
using MyTeam.Models.Dto;
using MyTeam.ViewModels.News;

namespace MyTeam.Services.Domain
{
    public interface IArticleService
    {
        IEnumerable<Article> Get(string clubId, int skip, int take);
        Article Get(Guid articleId);
        IEnumerable<SimpleArticleDto> GetSimple(string clubId, int take);
        Article CreateOrUpdate(EditArticleViewModel model, string clubId, Guid authorId);
        void Delete(Guid articleId);
    }
}