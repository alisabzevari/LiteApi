using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace LiteApi
{
    public class LiteApiController<TId, TDto, TEntity, TQueryDescriptor> : ApiController
        where TDto : new()
        where TEntity : class
    {
        protected readonly IPersistenceService PersistenceService;
        private bool _mappingDefined;

        public LiteApiController(IPersistenceService persistenceService)
        {
            PersistenceService = persistenceService;
            TryCreateMappings();
        }

        public virtual IHttpActionResult Get([FromUri] TQueryDescriptor queryDescriptor)
        {
            var queryable = PersistenceService.Query<TEntity>();
            var result = PerformQuery(queryable, queryDescriptor);
            return Ok(result.Project().To<TDto>());
        }

        public virtual IHttpActionResult Get([FromUri] TId id)
        {
            var entity = PersistenceService.GetById<TEntity>(id);
            if (entity == null)
                return NotFound();
            return Ok(MapToDto(entity));
        }
        public virtual IHttpActionResult Post(TDto dto)
        {
            var entity = MapToEntity(dto);
            PersistenceService.Add(entity);
            return Created(GetLocation(entity), MapToDto(entity));
        }
        public IHttpActionResult Put(TId id, [FromBody] TDto dto)
        {
            var oldEntity = PersistenceService.GetById<TEntity>(id);
            if (oldEntity == null)
                return NotFound();
            Merge(dto, ref oldEntity);
            PersistenceService.Update(oldEntity);
            return Ok();
        }
        public IHttpActionResult Delete(TId id)
        {
            var entity = PersistenceService.GetById<TEntity>(id);
            if (entity == null)
                return NotFound();
            PersistenceService.Remove(entity);
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
        private IQueryable<TEntity> PerformQuery(IQueryable<TEntity> queryable, TQueryDescriptor queryDescriptor)
        {
            var qb = new QueryBuilder<TEntity, TDto, TQueryDescriptor>(queryable, queryDescriptor);
            return qb.Execute();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PersistenceService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
