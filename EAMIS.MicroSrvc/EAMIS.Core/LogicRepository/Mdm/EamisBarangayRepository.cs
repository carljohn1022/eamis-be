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
    public class EamisBarangayRepository : IEamisBarangayReporsitory
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisBarangayRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        private EAMISBARANGAY MapToEntity(EamisBarangayDTO item)
        {
            if (item == null) return new EAMISBARANGAY();
            return new EAMISBARANGAY
            {
                PROVINCE_CODE = item.ProvinceCode,
                MUNICIPALITY_CODE = item.MunicipalityCode,
                BRGY_DESCRIPTION = item.BrgyDescription,
                REGION_CODE = item.RegionCode,
                BRGY_CODE = item.BrgyCode,
            };
        }
        public async Task<List<EAMISBARANGAY>> ListBarangay(string searchValue)
        {
            var result = _ctx.EAMIS_BARANGAY.AsNoTracking().Where(x => x.BRGY_DESCRIPTION == searchValue).ToList();
            return result;
        }
        public async Task<DataList<EamisBarangayDTO>> List(EamisBarangayDTO filter,PageConfig config)
        {
            IQueryable<EAMISBARANGAY> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query,resolved_size,resolved_index);
            return new DataList<EamisBarangayDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
          
        }
        public async Task<DataList<EamisBarangayDTO>> PublicSearch(string SearchType, string SearchValue,PageConfig config)
        {

            IQueryable<EAMISBARANGAY> query = null; 
            if (SearchType == "Barangay Description") 
            {
                query = _ctx.EAMIS_BARANGAY.AsNoTracking().Where(x => x.BRGY_DESCRIPTION.Contains(SearchValue)).AsQueryable();
               
            }
            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged =  PagedQueryForSearch(query, resolved_size, resolved_index);
            return new DataList<EamisBarangayDTO>
            {
                Count = await paged.CountAsync(),
                Items = await MapToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EAMISBARANGAY> FilteredEntities(EamisBarangayDTO filter, IQueryable<EAMISBARANGAY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISBARANGAY>(true);
           
            if (filter.BrgyCode != null && filter.BrgyCode != 0)
                predicate = predicate.And(x => x.BRGY_CODE == filter.BrgyCode);
            if (filter.RegionCode != null && filter.RegionCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);
            if (filter.MunicipalityCode != null && filter.MunicipalityCode != 0)
                predicate = predicate.And(x => x.MUNICIPALITY_CODE == filter.MunicipalityCode);
            if (filter.ProvinceCode != null && filter.ProvinceCode != 0)
                predicate = predicate.And(x => x.PROVINCE_CODE == filter.ProvinceCode);
            if (filter.BrgyDescription != null && !string.IsNullOrEmpty(filter.BrgyDescription))
                predicate = predicate.And(x => x.BRGY_DESCRIPTION == filter.BrgyDescription);
            var query = custom_query ?? _ctx.EAMIS_BARANGAY;
            return query.Where(predicate);
        }

        public IQueryable<EAMISBARANGAY> PagedQueryForSearch(IQueryable<EAMISBARANGAY> query,int resolved_size,int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public IQueryable<EAMISBARANGAY> PagedQuery(IQueryable<EAMISBARANGAY> query,int resolved_size,int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisBarangayDTO> QueryToDTO(IQueryable<EAMISBARANGAY> query)
        {
            return query.Select(x => new EamisBarangayDTO
            {
                RegionCode = x.REGION_CODE,
                ProvinceCode = x.PROVINCE_CODE,
                MunicipalityCode = x.MUNICIPALITY_CODE,
                BrgyCode = x.BRGY_CODE,
                BrgyDescription = x.BRGY_DESCRIPTION.ToUpper()
            });
        }
        private IQueryable<EamisBarangayDTO> MapToDTO(IQueryable<EAMISBARANGAY> query)
        {
            return query.Select(x => new EamisBarangayDTO {
            BrgyCode = x.BRGY_CODE,
            RegionCode = x.REGION_CODE,
            ProvinceCode = x.PROVINCE_CODE,
            MunicipalityCode = x.MUNICIPALITY_CODE,
            BrgyDescription = x.BRGY_DESCRIPTION
            });
        }

        

    }
}
