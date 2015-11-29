using System;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public interface IRepository<T> {

        IQueryable<T> Get();
        void Add(params T[] entities);
        void Delete(params T[] entities);
        T GetSingle(Guid id);
        void CommitChanges();
    }
}