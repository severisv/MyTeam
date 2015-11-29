using System;
using System.Linq;
using Microsoft.Data.Entity;
using MyTeam.Models;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {

        private readonly ApplicationDbContext _dbContext;


        public EfRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
        public IQueryable<TEntity> Get()
        {
            return _dbContext.Set<TEntity>();
        }

        public void Add(params TEntity[] entities)
        {
            _dbContext.Add(entities.Cast<TEntity>());
        }

        public void Delete(params TEntity[] entities)
        {
            _dbContext.Remove(entities);
        }

        public void CommitChanges()
        {
            _dbContext.SaveChanges();
        }


        public TEntity GetSingle(Guid id)
        {
            return _dbContext.Set<TEntity>().Single(e => e.Id == id);
        }
       
    }
}