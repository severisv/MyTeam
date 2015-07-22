using System;
using System.Collections.Generic;
using System.Linq;
using MyTeam.Models.Domain;

namespace MyTeam.Services.Repositories
{
    public class TestRepository
    {
        private readonly Dictionary<Type, List<Entity>> _repositories;

        public TestRepository()
        {
            _repositories = new Dictionary<Type, List<Entity>>();
            TestData.Addto(this);
        }

        public void Add<TType>(params TType[] entities) where TType : Entity
        {
            List<Entity> entityList;
            if (_repositories.TryGetValue(typeof(TType), out entityList))
            {
                entityList.AddRange(entities);
            }
            else
            {
                _repositories.Add(typeof(TType), new List<Entity>(entities));
            }
        }


        public void Remove<TType>(params TType[] entities) where TType : Entity
        {
            List<Entity> entityList;
            if (_repositories.TryGetValue(typeof(TType), out entityList))
            {
                entityList = entityList.Where(e => entities.All(entity => entity.Id != e.Id)).ToList();
                _repositories[typeof(TType)] = entityList;
            }
        }

        public TType[] Get<TType>() where TType : Entity
        {
            List<Entity> entityList;
            if (_repositories.TryGetValue(typeof(TType), out entityList))
            {
                return entityList.OfType<TType>().ToArray();
            }
            else
            {

                return new TType[] { };
            }
        }

        public TType GetSingle<TType>(Guid id) where TType : Entity
        {
            List<Entity> entityList;
            if (_repositories.TryGetValue(typeof(TType), out entityList))
            {
                return entityList.OfType<TType>().Single(e => e.Id == id);
            }
            throw new Exception("Repoet finnes ikke");

        }

    }
}