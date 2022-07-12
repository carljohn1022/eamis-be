using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Classification
{
    public class EamisClassificationRepository : IEamisClassificationRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisClassificationRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISCLASSIFICATION>> ListClassifications(string searchValue)
        {
            var result = _ctx.EAMIS_CLASSIFICATION.AsNoTracking().Where(x => x.NAME_CLASSIFICATION == searchValue).ToList();
            return result;
        }
        public async Task<DataList<EamisClassificationDTO>> List(EamisClassificationDTO filter, PageConfig config)
        {
            IQueryable<EAMISCLASSIFICATION> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisClassificationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisClassificationDTO> QueryToDTO(IQueryable<EAMISCLASSIFICATION> query)
        {
            return query.Select(x => new EamisClassificationDTO
            {
                Id = x.ID,
                NameClassification = x.NAME_CLASSIFICATION
            });
        }

        private IQueryable<EAMISCLASSIFICATION> FilteredEntities(EamisClassificationDTO filter, IQueryable<EAMISCLASSIFICATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISCLASSIFICATION>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.NameClassification != null && !string.IsNullOrEmpty(filter.NameClassification))
                predicate = predicate.And(x => x.NAME_CLASSIFICATION == filter.NameClassification);
            var query = custom_query ?? _ctx.EAMIS_CLASSIFICATION;
            return query.Where(predicate);
        }

        public IQueryable<EAMISCLASSIFICATION> PagedQuery(IQueryable<EAMISCLASSIFICATION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisClassificationDTO> Insert(EamisClassificationDTO item)
        {
            EAMISCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISCLASSIFICATION MapToEntity(EamisClassificationDTO item)
        {
            if (item == null) return new EAMISCLASSIFICATION();
            return new EAMISCLASSIFICATION
            {
                ID = item.Id,
                NAME_CLASSIFICATION = item.NameClassification
            };
        }

        public async Task<EamisClassificationDTO> Update(EamisClassificationDTO item, int Id)
        {
            EAMISCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisClassificationDTO> Delete(EamisClassificationDTO item, int Id)
        {
            EAMISCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
