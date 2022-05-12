using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisPropertySuppliesRepository : IEamisPropertySuppliesRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisPropertySuppliesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisPropertySuppliesDTO> Delete(EamisPropertySuppliesDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPROPERTYDETAILS MapToEntity(EamisPropertySuppliesDTO item)
        {
            if (item == null) return new EAMISPROPERTYDETAILS();
            return new EAMISPROPERTYDETAILS
            {
                ID = item.Id,
               
                UNIT_COST = item.Unit_Cost,
                IS_STOCKABLE = item.Is_Stockable,
                BRAND = item.Brand,
                MODEL_NO = item.Model_No

            };
        }

        public async Task<EamisPropertySuppliesDTO> Insert(EamisPropertySuppliesDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisPropertySuppliesDTO>> List(EamisPropertySuppliesDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYDETAILS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertySuppliesDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisPropertySuppliesDTO> QueryToDTO(IQueryable<EAMISPROPERTYDETAILS> query)
        {
            return query.Select(x => new EamisPropertySuppliesDTO
            {
                Id = x.ID,
              
                Unit_Cost = x.UNIT_COST,
                Is_Stockable = x.IS_STOCKABLE,
                Brand = x.BRAND,
                Model_No = x.MODEL_NO
            });
        }

        private IQueryable<EAMISPROPERTYDETAILS> PagedQuery(IQueryable<EAMISPROPERTYDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        private IQueryable<EAMISPROPERTYDETAILS> FilteredEntities(EamisPropertySuppliesDTO filter, IQueryable<EAMISPROPERTYDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
         
         
            if (filter.Unit_Cost != null && filter.Unit_Cost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.Unit_Cost);
            if (filter.Is_Stockable != null && filter.Is_Stockable != false)
                predicate = predicate.And(x => x.IS_STOCKABLE == filter.Is_Stockable);
            if (!string.IsNullOrEmpty(filter.Brand)) predicate = (strict)
                    ? predicate.And(x => x.BRAND.ToLower() == filter.Brand.ToLower())
                    : predicate.And(x => x.BRAND.Contains(filter.Brand.ToLower()));
            if (!string.IsNullOrEmpty(filter.Model_No)) predicate = (strict)
                    ? predicate.And(x => x.MODEL_NO.ToLower() == filter.Model_No.ToLower())
                    : predicate.And(x => x.MODEL_NO.Contains(filter.Model_No.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_DETAILS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertySuppliesDTO> Update(EamisPropertySuppliesDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
