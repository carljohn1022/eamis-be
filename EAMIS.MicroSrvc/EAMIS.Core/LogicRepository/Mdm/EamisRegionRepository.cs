using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
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

namespace EAMIS.Core.LogicRepository
{
    public class EamisRegionRepository : IEamisRegionRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisRegionRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        private EAMISREGION MapToEntity(EamisRegionDTO item)
        {
            if (item == null) return new EAMISREGION();
            return new EAMISREGION
            {
             PSGCODE = item.PsgCode,
             REGION_CODE = item.RegionCode,
             REGION_DESCRIPTION = item.RegionDescription
            };
        }   
        public async Task<DataList<EamisRegionDTO>> List(EamisRegionDTO filter,PageConfig config)
        {
            IQueryable<EAMISREGION> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query,resolved_size,resolved_index);
            return new DataList<EamisRegionDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EAMISREGION> FilteredEntities(EamisRegionDTO filter, IQueryable<EAMISREGION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISREGION>(true);
           
            if (filter.PsgCode != null && !string.IsNullOrEmpty(filter.PsgCode))
                predicate = predicate.And(x => x.PSGCODE == filter.PsgCode);
            if (filter.RegionCode != null && filter.RegionCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.RegionDescription != null && !string.IsNullOrEmpty(filter.RegionDescription))
                predicate = predicate.And(x => x.REGION_DESCRIPTION == filter.RegionDescription);
            var query = custom_query ?? _ctx.EAMIS_REGION;
            return query.Where(predicate);
        }

        public IQueryable<EAMISREGION> PagedQuery(IQueryable<EAMISREGION> query,int resolved_size,int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisRegionDTO> QueryToDTO(IQueryable<EAMISREGION> query)
        {
            return query.Select(x => new EamisRegionDTO
            {
               PsgCode = x.PSGCODE,
               RegionCode = x.REGION_CODE,
               RegionDescription = x.REGION_DESCRIPTION,
            });
        }
    }
}
