using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteApi
{
    public interface IPersistenceService : IDisposable
    {
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;
        TEntity GetById<TEntity>(object id) where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity: class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
    }
}
