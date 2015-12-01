using System;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public interface IRepository<T> {

        IQueryable<T> Get();
        T GetSingle(Guid id);
        void CommitChanges();
    }
}