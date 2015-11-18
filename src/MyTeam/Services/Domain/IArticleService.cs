using System;
using System.Collections.Generic;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Domain
{
    public interface IArticleService
    {
        IEnumerable<Article> Get(string clubId, int skip, int take);
        Article Get(Guid articleId);
    }
}