using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NHibernate;

namespace StrongApi
{
    public class NhStrongApiController<TDto, TEntity> : NhApiController
    {
        public NhStrongApiController(ISession session)
            : base(session)
        {
        }

        public virtual IHttpActionResult Get(object id)
        {
            var entity = Session.Get<TEntity>(id);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        public virtual IHttpActionResult Post(TDto dto)
        {
            var entity = MapToEntity(dto);
            Session.Save(entity);
            TryCommit();
            return Created(GetLocation(entity), entity);
        }
        public IHttpActionResult Put(object id, [FromBody] TDto dto)
        {
            var oldEntity = Session.Get<TEntity>(id);
            if (oldEntity == null)
                return NotFound();
            ReplaceEntityData(ref oldEntity, dto);
            Session.Update(oldEntity);
            TryCommit();
            return Ok();
        }

        public IHttpActionResult Delete(object id)
        {
            var entity = Session.Get<TEntity>(id);
            if (entity == null)
                return NotFound();
            Session.Delete(entity);
            TryCommit();
            return Ok();
        }

        protected virtual void ReplaceEntityData(ref TEntity oldEntity, TDto dto)
        {
            throw new NotImplementedException();
        }

        protected virtual Uri GetLocation(TEntity entity)
        {
            throw new NotImplementedException();
        }

        protected virtual TEntity MapToEntity(TDto dto)
        {
            throw new NotImplementedException();
        }

    }
}
