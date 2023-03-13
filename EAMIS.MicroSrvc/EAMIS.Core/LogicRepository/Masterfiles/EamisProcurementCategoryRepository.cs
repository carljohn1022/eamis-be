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
    public class EamisProcurementCategoryRepository : IEamisProcurementCategoryRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisProcurementCategoryRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISPROCUREMENTCATEGORY>> ListAllProcurements()
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROCUREMENTCATEGORY.AsNoTracking().ToList()).ConfigureAwait(false);
            return result;
        }

        public async Task<bool> InsertFromExcel(List<EamisProcurementCategoryDTO> Items)
        {
            List<EAMISPROCUREMENTCATEGORY> lstProcurement = new List<EAMISPROCUREMENTCATEGORY>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISPROCUREMENTCATEGORY objProcurement = MapToEntity(Items[intItems]);

                    lstProcurement.Add(objProcurement);
                }
                _ctx.EAMIS_PROCUREMENTCATEGORY.AddRange(lstProcurement);
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
                bolerror = false;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.InnerException.Message;
            }
            return HasError;
        }
        public async Task<EamisProcurementCategoryDTO> InsertFromExcel(EamisProcurementCategoryDTO item)
        {
            try
            {
                EAMISPROCUREMENTCATEGORY data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return item;
        }

        public async Task<EamisProcurementCategoryDTO> Delete(EamisProcurementCategoryDTO item)
        {
            EAMISPROCUREMENTCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPROCUREMENTCATEGORY MapToEntity(EamisProcurementCategoryDTO item)
        {
            if (item == null) return new EAMISPROCUREMENTCATEGORY();
            return new EAMISPROCUREMENTCATEGORY
            {
                ID = item.Id,
                PROCUREMENT_DESCRIPTION = item.ProcurementDescription,
                IS_ACTIVE = item.IsActive,
                USER_STAMP = item.UserStamp
            };
        }

        public async Task<EamisProcurementCategoryDTO> Insert(EamisProcurementCategoryDTO item)
        {
            EAMISPROCUREMENTCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisProcurementCategoryDTO>> List(EamisProcurementCategoryDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROCUREMENTCATEGORY> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisProcurementCategoryDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisProcurementCategoryDTO> QueryToDTO(IQueryable<EAMISPROCUREMENTCATEGORY> query)
        {
            return query.Select(x => new EamisProcurementCategoryDTO
            {
                Id = x.ID,
                ProcurementDescription = x.PROCUREMENT_DESCRIPTION,
                IsActive = x.IS_ACTIVE
            });
        }

        private IQueryable<EAMISPROCUREMENTCATEGORY> FilteredEntities(EamisProcurementCategoryDTO filter, IQueryable<EAMISPROCUREMENTCATEGORY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROCUREMENTCATEGORY>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.ProcurementDescription)) predicate = (strict)
                    ? predicate.And(x => x.PROCUREMENT_DESCRIPTION.ToLower() == filter.ProcurementDescription.ToLower())
                    : predicate.And(x => x.PROCUREMENT_DESCRIPTION.ToLower() == filter.ProcurementDescription.ToLower());
            var query = custom_query ?? _ctx.EAMIS_PROCUREMENTCATEGORY;
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROCUREMENTCATEGORY> PagedQuery(IQueryable<EAMISPROCUREMENTCATEGORY> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x=>x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisProcurementCategoryDTO> Update(int Id, EamisProcurementCategoryDTO item)
        {
            EAMISPROCUREMENTCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisProcurementCategoryDTO>> SearchProcurementCategory(string searchType, string searchValue)
        {
            IQueryable<EAMISPROCUREMENTCATEGORY> query = null;
           
                query = _ctx.EAMIS_PROCUREMENTCATEGORY.AsNoTracking().Where(x => x.PROCUREMENT_DESCRIPTION.Contains(searchValue)).AsQueryable();
         
            
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisProcurementCategoryDTO>
            {
                Count = await paged.CountAsync(),
                Items = await MapToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EamisProcurementCategoryDTO> MapToDTO(IQueryable<EAMISPROCUREMENTCATEGORY> query)
        {
            return query.Select(x => new EamisProcurementCategoryDTO
            {
                Id = x.ID,
                ProcurementDescription = x.PROCUREMENT_DESCRIPTION
            });
        }

        private IQueryable<EAMISPROCUREMENTCATEGORY> PagedQueryForSearch(IQueryable<EAMISPROCUREMENTCATEGORY> query)
        {
            return query;
        }

        public Task<bool> ValidateExistingDesc(string procurementDescription)
        {
            return _ctx.EAMIS_PROCUREMENTCATEGORY.AsNoTracking().AnyAsync(x => x.PROCUREMENT_DESCRIPTION == procurementDescription);
        }

        public Task<bool> ValidateExistingDescUpdate(int id, string procurementDescription)
        {
            return _ctx.EAMIS_PROCUREMENTCATEGORY.AsNoTracking().AnyAsync(x => x.ID == id && x.PROCUREMENT_DESCRIPTION == procurementDescription);
        }
    }
}
