using EAMIS.Common.DTO;
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
    public class EamisProvinceRepository : IEamisProvinceRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;

        public EamisProvinceRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

      
        private EAMISPROVINCE MapToEntity(EamisProvinceDTO item)
        {
            if (item == null) return new EAMISPROVINCE();
            return new EAMISPROVINCE
            {
               PSGCCODE = item.Psgccode,
               PROVINCE_CODE = item.ProvinceCode,
               PROVINCE_DESCRITION = item.ProvinceDescription,
               REGION_CODE = item.RegionCode,
            };
        }
        public async Task<List<EAMISPROVINCE>> ListProvince(string searchValue)
        {
            var result = _ctx.EAMIS_PROVINCE.AsNoTracking().Where(x => x.PROVINCE_DESCRITION == searchValue).ToList();
            return result;
        }
        public async Task<DataList<EamisProvinceDTO>> List(EamisProvinceDTO filter,PageConfig config)
        {
            IQueryable<EAMISPROVINCE> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisProvinceDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EAMISPROVINCE> FilteredEntities(EamisProvinceDTO filter, IQueryable<EAMISPROVINCE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROVINCE>(true);
            if (filter.Psgccode != null && !string.IsNullOrEmpty(filter.Psgccode))
                predicate = predicate.And(x => x.PSGCCODE == filter.Psgccode);
            if (filter.RegionCode != null && filter.RegionCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.ProvinceDescription != null && !string.IsNullOrEmpty(filter.ProvinceDescription))
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.ProvinceCode != null && filter.ProvinceCode != 0)
                predicate = predicate.And(x => x.PROVINCE_CODE == filter.ProvinceCode);
            var query = custom_query ?? _ctx.EAMIS_PROVINCE;
            return query.Where(predicate);
        }

        public IQueryable<EAMISPROVINCE> PagedQuery(IQueryable<EAMISPROVINCE> query,int resolved_size,int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisProvinceDTO> QueryToDTO(IQueryable<EAMISPROVINCE> query)
        {
            return query.Select(x => new EamisProvinceDTO
            {
                Psgccode = x.PSGCCODE,
                RegionCode = x.REGION_CODE,
                ProvinceCode = x.PROVINCE_CODE,
                ProvinceDescription = x.PROVINCE_DESCRITION
                
            });
        }
    }
}
