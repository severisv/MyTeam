using System;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public interface IRepository<T> where T : Entity{

        IQueryable<T> Get(params Guid[] ids);
        IQueryable<T> Get();
        void Add(params T[] entities);
        void Delete(params T[] entities);
        T GetSingle(Guid id);
    }
}