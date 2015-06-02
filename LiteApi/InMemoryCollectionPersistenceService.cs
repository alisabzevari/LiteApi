using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    public class InMemoryCollectionPersistenceService<T> : IPersistenceService where T : class
    {
        private readonly ICollection<T> _collection;

        public InMemoryCollectionPersistenceService(ICollection<T> collection)
        {
            _collection = collection;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return _collection.AsQueryable() as IQueryable<TEntity>;
        }

        public TEntity GetById<TEntity>(object id) where TEntity : class
        {
            return _collection.SingleOrDefault(x => id.Equals(x.GetId())) as TEntity;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _collection.Add(entity as T);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {

        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _collection.Remove(entity as T);
        }

        public void Dispose()
        {
            
        }


    }
}
