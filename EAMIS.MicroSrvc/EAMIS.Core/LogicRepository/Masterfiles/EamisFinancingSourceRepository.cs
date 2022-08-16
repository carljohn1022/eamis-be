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
    public class EamisFinancingSourceRepository : IEamisFinancingSourceRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisFinancingSourceRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISFINANCINGSOURCE>> ListFinancingSource(string searchValue)
        {
            var result = _ctx.EAMIS_FINANCING_SOURCE.AsNoTracking().Where(x => x.FINANCING_SOURCE_NAME == searchValue).ToList();
            return result;
        }
        public async Task<List<EAMISFINANCINGSOURCE>> ListFinancingSourceById(int financeSourceId)
        {
            var result = _ctx.EAMIS_FINANCING_SOURCE.AsNoTracking().Where(x => x.ID == financeSourceId).ToList();
            return result;
        }
        public async Task<DataList<EamisFinancingSourceDTO>> List(EamisFinancingSourceDTO filter, PageConfig config)
        {
            IQueryable<EAMISFINANCINGSOURCE> query = FilteredEntites(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisFinancingSourceDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISFINANCINGSOURCE> PagedQuery(IQueryable<EAMISFINANCINGSOURCE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisFinancingSourceDTO> QueryToDTO(IQueryable<EAMISFINANCINGSOURCE> query)
        {
            return query.Select(x => new EamisFinancingSourceDTO
            {
                Id = x.ID,
                FinancingSourceName = x.FINANCING_SOURCE_NAME
            });
        }

        private IQueryable<EAMISFINANCINGSOURCE> FilteredEntites(EamisFinancingSourceDTO filter, IQueryable<EAMISFINANCINGSOURCE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISFINANCINGSOURCE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (!string.IsNullOrEmpty(filter.FinancingSourceName)) predicate = (strict)
                     ? predicate.And(x => x.FINANCING_SOURCE_NAME.ToLower() == filter.FinancingSourceName.ToLower())
                     : predicate.And(x => x.FINANCING_SOURCE_NAME.ToLower() == filter.FinancingSourceName.ToLower());

            var query = custom_query ?? _ctx.EAMIS_FINANCING_SOURCE;
            return query.Where(predicate);
        }
    }
}
