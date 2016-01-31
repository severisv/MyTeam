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
        PagedList<ArticleViewModel> Get(Guid clubId, int skip, int take);
        ArticleViewModel Get(Guid articleId);
        PagedList<SimpleArticleDto> GetSimple(Guid clubId, int take);
        ArticleViewModel CreateOrUpdate(EditArticleViewModel model, Guid clubId, Guid authorId);
        void Delete(Guid articleId);

        IEnumerable<CommentViewModel> GetComments(Guid articleId);
    }
}