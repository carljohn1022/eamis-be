using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisEnvironmentalImpactsRepository : IEamisEnvironmentalImpactsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;

        public EamisEnvironmentalImpactsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                            : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisEnvironmentalImpactsDTO> Delete(EamisEnvironmentalImpactsDTO item)
        {
            EAMISENVIRONMENTALIMPACTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISENVIRONMENTALIMPACTS MapToEntity(EamisEnvironmentalImpactsDTO item)
        {
            if (item == null) return new EAMISENVIRONMENTALIMPACTS();
            return new EAMISENVIRONMENTALIMPACTS
            {
                ID = item.Id,
                IMPACT_DESCPRIPTION = item.ImpactDescription
            };
        }

        public async Task<EamisEnvironmentalImpactsDTO> Insert(EamisEnvironmentalImpactsDTO item)
        {
            EAMISENVIRONMENTALIMPACTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisEnvironmentalImpactsDTO>> List(EamisEnvironmentalImpactsDTO filter, PageConfig config)
        {
            IQueryable<EAMISENVIRONMENTALIMPACTS> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisEnvironmentalImpactsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };

        }

        private IQueryable<EamisEnvironmentalImpactsDTO> QueryToDTO(IQueryable<EAMISENVIRONMENTALIMPACTS> query)
        {
            return query.Select(x => new EamisEnvironmentalImpactsDTO
            {
                Id = x.ID,
                ImpactDescription = x.IMPACT_DESCPRIPTION
            });
        }

        private IQueryable<EAMISENVIRONMENTALIMPACTS> PagedQuery(IQueryable<EAMISENVIRONMENTALIMPACTS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISENVIRONMENTALIMPACTS> FilteredEntities(EamisEnvironmentalImpactsDTO filter, IQueryable<EAMISENVIRONMENTALIMPACTS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISENVIRONMENTALIMPACTS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.ImpactDescription)) predicate = (strict)
                    ? predicate = predicate.And(x => x.IMPACT_DESCPRIPTION.ToLower() == filter.ImpactDescription.ToLower())
                    : predicate = predicate.And(x => x.IMPACT_DESCPRIPTION.ToLower() == filter.ImpactDescription.ToLower());
            var query = custom_query ?? _ctx.EAMIS_ENVIRONMENTALIMPACTS;
            return query.Where(predicate);
        }

        public async Task<EamisEnvironmentalImpactsDTO> Update(EamisEnvironmentalImpactsDTO item)
        {
            EAMISENVIRONMENTALIMPACTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
