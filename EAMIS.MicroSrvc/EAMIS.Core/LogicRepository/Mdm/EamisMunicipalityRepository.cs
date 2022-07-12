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
    public class EamisMunicipalityRepository : IEamisMunicipalityRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;

        public EamisMunicipalityRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        private EAMISMUNICIPALITY MapToEntity(EamisMunicipalityDTO item)
        {
            if (item == null) return new EAMISMUNICIPALITY();
            return new EAMISMUNICIPALITY
            {
                
                PSGCODE = item.Psgcode,
                PROVINCE_CODE = item.ProvinceCode,
                MUNICIPALITY_CODE = item.CityMunicipalityCode,
               CITY_MUNICIPALITY_DESCRIPTION = item.CityMunicipalityDescription,
                REGION_CODE = item.RegionCode,
            };
        }
        public async Task<List<EAMISMUNICIPALITY>> ListMunicipality(string searchValue)
        {
            var result = _ctx.EAMIS_MUNICIPALITY.AsNoTracking().Where(x => x.CITY_MUNICIPALITY_DESCRIPTION == searchValue).ToList();
            return result;
        }
        public async Task<DataList<EamisMunicipalityDTO>> List(EamisMunicipalityDTO filter,PageConfig config)
        {
            IQueryable<EAMISMUNICIPALITY> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query,resolved_size,resolved_index);
            return new DataList<EamisMunicipalityDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EAMISMUNICIPALITY> FilteredEntities(EamisMunicipalityDTO filter, IQueryable<EAMISMUNICIPALITY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISMUNICIPALITY>(true);
          
            if (filter.Psgcode != null && !string.IsNullOrEmpty(filter.Psgcode))
                predicate = predicate.And(x => x.PSGCODE == filter.Psgcode);
            if (filter.RegionCode != null && filter.RegionCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.CityMunicipalityCode != null && filter.CityMunicipalityCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.ProvinceCode != null && filter.ProvinceCode != 0)
                predicate = predicate.And(x => x.PROVINCE_CODE == filter.ProvinceCode);
            if (filter.CityMunicipalityDescription != null && !string.IsNullOrEmpty(filter.CityMunicipalityDescription))
                predicate = predicate.And(x => x.CITY_MUNICIPALITY_DESCRIPTION == filter.CityMunicipalityDescription);
            var query = custom_query ?? _ctx.EAMIS_MUNICIPALITY;
            return query.Where(predicate);
        }

        public IQueryable<EAMISMUNICIPALITY> PagedQuery(IQueryable<EAMISMUNICIPALITY> query,int resolved_size,int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisMunicipalityDTO> QueryToDTO(IQueryable<EAMISMUNICIPALITY> query)
        {
            return query.Select(x => new EamisMunicipalityDTO
            {
             
                Psgcode = x.PSGCODE,
                RegionCode = x.REGION_CODE,
                ProvinceCode = x.PROVINCE_CODE,
               CityMunicipalityCode = x.MUNICIPALITY_CODE,
               CityMunicipalityDescription = x.CITY_MUNICIPALITY_DESCRIPTION
            });
        }
    }
}
