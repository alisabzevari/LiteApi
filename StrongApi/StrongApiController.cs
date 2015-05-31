using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using AutoMapper;
using NHibernate.Mapping;

namespace StrongApi
{
    public class StrongApiController<TId, TDto, TEntity, TQueryDescriptor> : ApiController where TDto : new()
    {
        protected readonly ICollection<TEntity> Collection;

        public StrongApiController(ICollection<TEntity> collection)
        {
            Mapper.CreateMap<TEntity, TDto>();
            Mapper.CreateMap<TDto, TEntity>();
            Collection = collection;
        }

        public virtual IHttpActionResult Get([FromUri] TId id)
        {
            var entity = FindEntityById(id);
            if (entity == null)
                return NotFound();
            return Ok(MapToDto(entity));
        }

        public virtual IHttpActionResult Post(TDto dto)
        {
            var entity = MapToEntity(dto);
            Collection.Add(entity);
            return Created(GetLocation(entity), MapToDto(entity));
        }

        public IHttpActionResult Put(TId id, [FromBody] TDto dto)
        {
            var oldEntity = FindEntityById(id);
            if (oldEntity == null)
                return NotFound();
            Merge(dto, ref oldEntity);
            return Ok();
        }
        public IHttpActionResult Delete(TId id)
        {
            var entity = FindEntityById(id);
            if (entity == null)
                return NotFound();
            Collection.Remove(entity);
            return Ok();
        }

        protected virtual TDto MapToDto(TEntity entity)
        {
            return Mapper.Map<TDto>(entity);
        }

        protected virtual TEntity MapToEntity(TDto dto)
        {
            return Mapper.Map<TEntity>(dto);
        }
        protected virtual Uri GetLocation(TEntity entity)
        {
            if (Request == null) 
                return new Uri("http://localhost/");
            return new Uri(Request.RequestUri.AbsoluteUri + "?id=" + entity.GetId());
        }
        private TEntity FindEntityById(TId id)
        {
            return Collection.SingleOrDefault(x => id.Equals(x.GetId()));
        }
        private void Merge(TDto dto, ref TEntity entity)
        {
            Mapper.Map(dto, entity);
        }
    }
}
