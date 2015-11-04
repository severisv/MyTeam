using System;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly TestRepository _testRepository;

        public GenericRepository(TestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public IQueryable<TEntity> Get(params Guid[] ids)
        {
            var entities = _testRepository.Get<TEntity>();
            if (ids.Any()) entities = entities.Where(e => ids.Any(id => id == e.Id)).ToArray();
            return entities.AsQueryable();

        }

        public IQueryable<TEntity> Get()
        {
            return _testRepository.Get<TEntity>().AsQueryable();
        }

        public void Add(params TEntity[] entities)
        {
            _testRepository.Add(entities);
        }

        public void Delete(params TEntity[] entities)
        {
            _testRepository.Remove(entities);
        }

        public TEntity GetSingle(Guid id)
        {
            try
            {
                return _testRepository.GetSingle<TEntity>(id);
            }
            catch (InvalidOperationException)
            {
                return _testRepository.Get<TEntity>().First();
            }
        }

        public void Update(TEntity entity)
        {
           

        }

        public void CommitChanges()
        {
            
        }
    }
}