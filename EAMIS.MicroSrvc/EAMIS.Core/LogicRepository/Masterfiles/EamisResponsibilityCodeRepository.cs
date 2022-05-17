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
    public class EamisResponsibilityCodeRepository : IEamisResponsibilityCodeRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisResponsibilityCodeRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxSizePage")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        private EAMISRESPONSIBILITYCODE MapToEntity(EamisResponsibilityCodeDTO item)
        {
            if (item == null) return new EAMISRESPONSIBILITYCODE();
            return new EAMISRESPONSIBILITYCODE
            {
                ID = item.Id,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                IS_ACTIVE = item.isActive
            };
        }
        public async Task<DataList<EamisResponsibilityCodeDTO>> List(EamisResponsibilityCodeDTO filter, PageConfig config)
        {
            IQueryable<EAMISRESPONSIBILITYCODE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisResponsibilityCodeDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EamisResponsibilityCodeDTO> QueryToDTO(IQueryable<EAMISRESPONSIBILITYCODE> query)
        {
            return query.Select(x => new EamisResponsibilityCodeDTO
            {
                Id = x.ID,
                Office = x.OFFICE,
                Department = x.DEPARTMENT,
                isActive = x.IS_ACTIVE

            });
        }
        public async Task<EamisResponsibilityCodeDTO> Insert(EamisResponsibilityCodeDTO item)
        {
            EAMISRESPONSIBILITYCODE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<DataList<EamisResponsibilityCodeDTO>> SearchResCenter(string type, string searchValue)
        {
            IQueryable<EAMISRESPONSIBILITYCODE> query = null;
            if (type == "Office")
            {
                query = _ctx.EAMIS_RESPONSIBILITY_CODE.AsNoTracking().Where(x => x.OFFICE.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_RESPONSIBILITY_CODE.AsNoTracking().Where(x => x.DEPARTMENT.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisResponsibilityCodeDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EAMISRESPONSIBILITYCODE> PagedQueryForSearch(IQueryable<EAMISRESPONSIBILITYCODE> query)
        {
            return query;
        }
        private IQueryable<EAMISRESPONSIBILITYCODE> PagedQuery(IQueryable<EAMISRESPONSIBILITYCODE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }
        private IQueryable<EAMISRESPONSIBILITYCODE> FilteredEntities(EamisResponsibilityCodeDTO filter, IQueryable<EAMISRESPONSIBILITYCODE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISRESPONSIBILITYCODE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                    ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                    : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_RESPONSIBILITY_CODE;
            return query.Where(predicate);
        }
        public async Task<EamisResponsibilityCodeDTO> Update(EamisResponsibilityCodeDTO item, int id)
        {
            EAMISRESPONSIBILITYCODE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public Task<bool> ValidateExistingCode(string office, string department)
        {
            return _ctx.EAMIS_RESPONSIBILITY_CODE.AsNoTracking().AnyAsync(x => x.OFFICE == office || x.DEPARTMENT == department);
        }
        public Task<bool> UpdateValidateExistingCode(string office, string department, int id)
        {
            return _ctx.EAMIS_RESPONSIBILITY_CODE.AsNoTracking().AnyAsync(x => x.OFFICE == office && x.DEPARTMENT == department || x.ID == id);
        }



    }
}

