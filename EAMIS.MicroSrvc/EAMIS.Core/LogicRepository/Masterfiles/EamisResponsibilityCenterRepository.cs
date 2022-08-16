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
    namespace EAMIS.Core.LogicRepository.Masterfiles
    {
        public class EamisResponsibilityCenterRepository : IEamisResponsibilityCenterRepository
        {
            private readonly EAMISContext _ctx;
            private readonly int _maxPageSize;
            public EamisResponsibilityCenterRepository(EAMISContext ctx)
            {
                _ctx = ctx;
                _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxSizePage")) ? 100
                    : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
            }
            private EAMISRESPONSIBILITYCENTER MapToEntity(EamisResponsibilityCenterDTO item)
            {
                item.responsibilityCenter = item.mainGroupCode + item.subGroupCode + item.officeCode + item.unitCode;
                if (item == null) return new EAMISRESPONSIBILITYCENTER();
                return new EAMISRESPONSIBILITYCENTER
                {

                    ID = item.Id,
                    MAIN_GROUP_CODE = item.mainGroupCode,
                    MAIN_GROUP_DESC = item.mainGroupDesc,
                    SUB_GROUP_CODE = item.subGroupCode,
                    SUB_GROUP_DESC = item.subGroupDesc,
                    OFFICE_CODE = item.officeCode,
                    OFFICE_DESC = item.officeDesc,
                    UNIT_CODE = item.unitCode,
                    UNIT_DESC = item.unitDesc,
                    IS_ACTIVE = item.isActive,
                    RESPONSIBILITY_CENTER = item.responsibilityCenter
                    
                };
            }
            public async Task<DataList<EamisResponsibilityCenterDTO>> List(EamisResponsibilityCenterDTO filter, PageConfig config)
            {
                IQueryable<EAMISRESPONSIBILITYCENTER> query = FilteredEntities(filter);

                string resolved_sort = config.SortBy ?? "Id";
                bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
                int resolved_size = config.Size ?? _maxPageSize;
                if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
                int resolved_index = config.Index ?? 1;

                var paged = PagedQuery(query, resolved_size, resolved_index);
                return new DataList<EamisResponsibilityCenterDTO>
                {
                    Count = await query.CountAsync(),
                    Items = await QueryToDTO(paged).ToListAsync(),

                };
            }
            private IQueryable<EamisResponsibilityCenterDTO> QueryToDTO(IQueryable<EAMISRESPONSIBILITYCENTER> query)
            {
                return query.Select(x => new EamisResponsibilityCenterDTO
                {
                    Id = x.ID,
                    mainGroupCode = x.MAIN_GROUP_CODE,
                    mainGroupDesc = x.MAIN_GROUP_DESC,
                    subGroupCode = x.SUB_GROUP_CODE,
                    subGroupDesc = x.SUB_GROUP_DESC,
                    officeCode = x.OFFICE_CODE,
                    officeDesc = x.OFFICE_DESC,
                    unitCode = x.UNIT_CODE,
                    unitDesc = x.UNIT_DESC,
                    responsibilityCenter = x.RESPONSIBILITY_CENTER,
                    isActive = x.IS_ACTIVE

                });
            }
            public async Task<EamisResponsibilityCenterDTO> Insert(EamisResponsibilityCenterDTO item)
            {
                EAMISRESPONSIBILITYCENTER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                return item;
            }
            public async Task<DataList<EamisResponsibilityCenterDTO>> SearchResCenter(string type, string searchValue)
            {
                IQueryable<EAMISRESPONSIBILITYCENTER> query = null;
                if (type == "Responsibility Center")
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.RESPONSIBILITY_CENTER.Contains(searchValue)).AsQueryable();
                }
                else if (type == "Main Group")
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.MAIN_GROUP_DESC.Contains(searchValue)).AsQueryable();
                }
                else if (type == "Sub Group")
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.SUB_GROUP_DESC.Contains(searchValue)).AsQueryable();
                }
                else if (type == "Office")
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.OFFICE_DESC.Contains(searchValue)).AsQueryable();
                }
                else if (type == "Unit")
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.UNIT_DESC.Contains(searchValue)).AsQueryable();
                }
                else
                {
                    query = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().Where(x => x.RESPONSIBILITY_CENTER.Contains(searchValue)).AsQueryable();
                }

                var paged = PagedQueryForSearch(query);
                return new DataList<EamisResponsibilityCenterDTO>
                {
                    Count = await paged.CountAsync(),
                    Items = await QueryToDTO(paged).ToListAsync()
                };

            }

            private IQueryable<EAMISRESPONSIBILITYCENTER> PagedQueryForSearch(IQueryable<EAMISRESPONSIBILITYCENTER> query)
            {
                return query;
            }
            private IQueryable<EAMISRESPONSIBILITYCENTER> PagedQuery(IQueryable<EAMISRESPONSIBILITYCENTER> query, int resolved_size, int resolved_index)
            {
                return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

            }
            private IQueryable<EAMISRESPONSIBILITYCENTER> FilteredEntities(EamisResponsibilityCenterDTO filter, IQueryable<EAMISRESPONSIBILITYCENTER> custom_query = null, bool strict = false)
            {
                var predicate = PredicateBuilder.New<EAMISRESPONSIBILITYCENTER>(true);
                if (filter.Id != null && filter.Id != 0)
                    predicate = predicate.And(x => x.ID == filter.Id);
                if (!string.IsNullOrEmpty(filter.mainGroupCode)) predicate = (strict)
                        ? predicate.And(x => x.MAIN_GROUP_CODE.ToLower() == filter.mainGroupCode.ToLower())
                        : predicate.And(x => x.MAIN_GROUP_CODE.Contains(filter.mainGroupCode.ToLower()));
                if (!string.IsNullOrEmpty(filter.mainGroupDesc)) predicate = (strict)
                         ? predicate.And(x => x.MAIN_GROUP_DESC.ToLower() == filter.mainGroupDesc.ToLower())
                         : predicate.And(x => x.MAIN_GROUP_DESC.Contains(filter.mainGroupDesc.ToLower()));
                if (!string.IsNullOrEmpty(filter.subGroupCode)) predicate = (strict)
                        ? predicate.And(x => x.SUB_GROUP_CODE.ToLower() == filter.subGroupCode.ToLower())
                        : predicate.And(x => x.SUB_GROUP_CODE.Contains(filter.subGroupCode.ToLower()));
                if (!string.IsNullOrEmpty(filter.subGroupDesc)) predicate = (strict)
                        ? predicate.And(x => x.SUB_GROUP_DESC.ToLower() == filter.subGroupDesc.ToLower())
                        : predicate.And(x => x.SUB_GROUP_DESC.Contains(filter.subGroupDesc.ToLower()));
                if (!string.IsNullOrEmpty(filter.officeCode)) predicate = (strict)
                         ? predicate.And(x => x.OFFICE_CODE.ToLower() == filter.officeCode.ToLower())
                         : predicate.And(x => x.OFFICE_CODE.Contains(filter.officeCode.ToLower()));
                if (!string.IsNullOrEmpty(filter.officeDesc)) predicate = (strict)
                         ? predicate.And(x => x.OFFICE_DESC.ToLower() == filter.officeDesc.ToLower())
                         : predicate.And(x => x.OFFICE_DESC.Contains(filter.officeDesc.ToLower()));
                if (!string.IsNullOrEmpty(filter.unitCode)) predicate = (strict)
                         ? predicate.And(x => x.UNIT_CODE.ToLower() == filter.unitCode.ToLower())
                         : predicate.And(x => x.UNIT_CODE.Contains(filter.unitCode.ToLower()));
                if (!string.IsNullOrEmpty(filter.unitDesc)) predicate = (strict)
                         ? predicate.And(x => x.UNIT_DESC.ToLower() == filter.unitDesc.ToLower())
                         : predicate.And(x => x.UNIT_DESC.Contains(filter.unitDesc.ToLower()));
                if (filter.isActive != null && filter.isActive != false)
                    predicate = predicate.And(x => x.IS_ACTIVE == filter.isActive);

                var query = custom_query ?? _ctx.EAMIS_RESPONSIBILITY_CENTER;
                return query.Where(predicate);
            }
            public async Task<EamisResponsibilityCenterDTO> Update(EamisResponsibilityCenterDTO item, int id)
            {
                EAMISRESPONSIBILITYCENTER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
                return item;
            }
            public Task<bool> ValidateExistingCode(string ResponsibilityCenter)
            {
                return _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().AnyAsync(x => x.RESPONSIBILITY_CENTER == ResponsibilityCenter);
            }
            public Task<bool> UpdateValidateExistingCode(string ResponsibilityCenter, int id)
            {
                return _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking().AnyAsync(x => x.RESPONSIBILITY_CENTER == ResponsibilityCenter && x.ID == id);
            }
        }

    }
}
