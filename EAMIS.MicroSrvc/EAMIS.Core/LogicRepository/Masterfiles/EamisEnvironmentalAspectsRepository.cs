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
    public class EamisEnvironmentalAspectsRepository : IEamisEnvironmentalAspectsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisEnvironmentalAspectsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                            : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisEnvironmentalAspectsDTO> Delete(EamisEnvironmentalAspectsDTO item)
        {
            EAMISSIGNIFICANTENVIRONMENTALASPECTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISSIGNIFICANTENVIRONMENTALASPECTS MapToEntity(EamisEnvironmentalAspectsDTO item)
        {
            if (item == null) return new EAMISSIGNIFICANTENVIRONMENTALASPECTS();
            return new EAMISSIGNIFICANTENVIRONMENTALASPECTS
            {
                ID = item.Id,
                ASPECT_DESCPRIPTION = item.AspectDescription
            };
        }

        public async Task<EamisEnvironmentalAspectsDTO> Update(EamisEnvironmentalAspectsDTO item)
        {
            EAMISSIGNIFICANTENVIRONMENTALASPECTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisEnvironmentalAspectsDTO> Insert(EamisEnvironmentalAspectsDTO item)
        {
            EAMISSIGNIFICANTENVIRONMENTALASPECTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisEnvironmentalAspectsDTO>> List(EamisEnvironmentalAspectsDTO filter, PageConfig config)
        {
            IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size,resolved_index);
            return new DataList<EamisEnvironmentalAspectsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisEnvironmentalAspectsDTO> QueryToDTO(IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> query)
        {
            return query.Select(x => new EamisEnvironmentalAspectsDTO
            {
                Id = x.ID,
                AspectDescription = x.ASPECT_DESCPRIPTION
            });
        }

        private IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> PagedQuery(IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> FilteredEntities(EamisEnvironmentalAspectsDTO filter, IQueryable<EAMISSIGNIFICANTENVIRONMENTALASPECTS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSIGNIFICANTENVIRONMENTALASPECTS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.AspectDescription)) predicate = (strict)
                    ? predicate = predicate.And(x => x.ASPECT_DESCPRIPTION.ToLower() == filter.AspectDescription.ToLower())
                    : predicate = predicate.And(x => x.ASPECT_DESCPRIPTION.ToLower() == filter.AspectDescription.ToLower());
            var query = custom_query ?? _ctx.EAMIS_ENVIRONMENTALASPECTS;
            return query.Where(predicate);
        }
    }
}
