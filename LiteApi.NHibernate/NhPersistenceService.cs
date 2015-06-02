using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;

namespace LiteApi.NHibernate
{
    public class NhPersistenceService: IPersistenceService
    {
        private readonly ISession _session;

        public NhPersistenceService(ISession session)
        {
            _session = session;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return _session.Query<TEntity>();
        }

        public TEntity GetById<TEntity>(object id) where TEntity : class
        {
            return _session.Get<TEntity>(id);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _session.Save(entity);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _session.Delete(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _session.Update(entity);
        }
        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
