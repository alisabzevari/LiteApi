using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;

namespace StrongApi
{
    public class StrongApiController<TId, TDto, TEntity, TQueryDescriptor> : ApiController where TDto : new()
    {
        protected readonly ICollection<TEntity> Collection;

        public StrongApiController(ICollection<TEntity> collection)
        {
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
            return Created(GetLocation(entity), entity);
        }

        public IHttpActionResult Put(TId id, [FromBody] TDto dto)
        {
            var oldEntity = FindEntityById(id);
            if (oldEntity == null)
                return NotFound();
            ReplaceEntityData(ref oldEntity, dto);
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
            Mapper.CreateMap<TEntity, TDto>();
            return Mapper.Map<TDto>(entity);
        }

        protected virtual TEntity MapToEntity(TDto dto)
        {
            Mapper.CreateMap<TDto, TEntity>();
            return Mapper.Map<TEntity>(dto);
        }
        private Uri GetLocation(TEntity entity)
        {
            throw new NotImplementedException();
        }
        private TEntity FindEntityById(TId id)
        {
            return Collection.SingleOrDefault(x => id.Equals(x.ExtractIdValue()));
        }
        private void ReplaceEntityData(ref TEntity oldEntity, TDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
