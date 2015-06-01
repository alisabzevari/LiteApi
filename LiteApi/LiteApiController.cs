using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using AutoMapper;

namespace LiteApi
{
    public class LiteApiController<TId, TDto, TEntity, TQueryDescriptor> : ApiController where TDto : new()
    {
        protected readonly ICollection<TEntity> Collection;
        private bool _mappingDefined;

        public LiteApiController(ICollection<TEntity> collection)
        {
            Collection = collection;
        }

        public virtual IHttpActionResult Get([FromUri] TQueryDescriptor queryDescriptor)
        {
            var queryable = Collection.AsQueryable();
            var result = PerformQuery(queryable, queryDescriptor);
            return Ok(result.Select(MapToDto));
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
        protected virtual void DefineMappings()
        {
            Mapper.CreateMap<TEntity, TDto>();
            Mapper.CreateMap<TDto, TEntity>();
        }
        private void TryCreateMappings()
        {
            if (!_mappingDefined)
            {
                DefineMappings();
                _mappingDefined = true;
            }
        }
        private TDto MapToDto(TEntity entity)
        {
            TryCreateMappings();
            return Mapper.Map<TDto>(entity);
        }
        private TEntity MapToEntity(TDto dto)
        {
            TryCreateMappings();
            return Mapper.Map<TEntity>(dto);
        }
        private void Merge(TDto dto, ref TEntity entity)
        {
            TryCreateMappings();
            Mapper.Map(dto, entity);
        }
        private Uri GetLocation(TEntity entity)
        {
#if DEBUG
            // just to make my tests work!
            if (Request == null) 
                return new Uri("http://localhost/");
#endif
            return new Uri(Request.RequestUri.AbsoluteUri + "?id=" + entity.GetId());
        }
        private TEntity FindEntityById(TId id)
        {
            return Collection.SingleOrDefault(x => id.Equals(x.GetId()));
        }
        private IEnumerable<TEntity> PerformQuery(IQueryable<TEntity> queryable, TQueryDescriptor queryDescriptor)
        {
            throw new NotImplementedException();
        }
    }
}
