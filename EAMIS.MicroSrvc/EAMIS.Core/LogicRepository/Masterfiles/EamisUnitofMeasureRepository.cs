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
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisUnitofMeasureRepository : IEamisUnitofMeasureRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisUnitofMeasureRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisUnitofMeasureDTO> Delete(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<bool> InsertFromExcel(List<EamisUnitofMeasureDTO> Items)
        {
            List<EAMISUNITOFMEASURE> lstUOM = new List<EAMISUNITOFMEASURE>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISUNITOFMEASURE objSupplier = MapToEntity(Items[intItems]);

                    lstUOM.Add(objSupplier);
                }
                _ctx.EAMIS_UNITOFMEASURE.AddRange(lstUOM);
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
        public async Task<EamisUnitofMeasureDTO> InsertFromExcel(EamisUnitofMeasureDTO item)
        {
            try
            {
                EAMISUNITOFMEASURE data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.InnerException.Message;
            }
            return item;
        }
        public async Task<List<EAMISUNITOFMEASURE>> ListAllIUnitOfMeasurement()
        {
            //IQueryable<EAMISUNITOFMEASURE> query = _ctx.EAMIS_UNITOFMEASURE;
            //var result = query.ToListAsync().GetAwaiter().GetResult();
            var result = _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().ToList();
            return result;
        }
        private EAMISUNITOFMEASURE MapToEntity(EamisUnitofMeasureDTO item)
        {
            if (item == null) return new EAMISUNITOFMEASURE();
            return new EAMISUNITOFMEASURE
            {
                ID = item.Id,
                SHORT_DESCRIPTION = item.Short_Description,
                UOM_DESCRIPTION = item.Uom_Description,
                IS_ACTIVE = item.isActive,
                USER_STAMP = item.UserStamp

            };
        }

        public async Task<EamisUnitofMeasureDTO> Insert(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisUnitofMeasureDTO>> List(EamisUnitofMeasureDTO filter, PageConfig config)
        {
            IQueryable<EAMISUNITOFMEASURE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisUnitofMeasureDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisUnitofMeasureDTO> QueryToDTO(IQueryable<EAMISUNITOFMEASURE> query)
        {
            return query.Select(x => new EamisUnitofMeasureDTO
            {
                Id = x.ID,
                Short_Description = x.SHORT_DESCRIPTION,
                Uom_Description = x.UOM_DESCRIPTION,
                isActive = x.IS_ACTIVE

            });
        }

        private IQueryable<EAMISUNITOFMEASURE> PagedQuery(IQueryable<EAMISUNITOFMEASURE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x=>x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        public async Task<DataList<EamisUnitofMeasureDTO>> SearchMeasure(string type, string SearchValue)
        {

            IQueryable<EAMISUNITOFMEASURE> query = null;
            if (type == "Short Description")
            {
                query = _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().Where(x => x.SHORT_DESCRIPTION.Contains(SearchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().Where(x => x.UOM_DESCRIPTION.Contains(SearchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisUnitofMeasureDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EAMISUNITOFMEASURE> PagedQueryForSearch(IQueryable<EAMISUNITOFMEASURE> query)
        {
            return query;
        }
        private IQueryable<EAMISUNITOFMEASURE> FilteredEntities(EamisUnitofMeasureDTO filter, IQueryable<EAMISUNITOFMEASURE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISUNITOFMEASURE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Uom_Description)) predicate = (strict)
                    ? predicate.And(x => x.UOM_DESCRIPTION.ToLower() == filter.Uom_Description.ToLower())
                    : predicate.And(x => x.UOM_DESCRIPTION.Contains(filter.Uom_Description.ToLower()));
            if (filter.isActive != null && filter.isActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.isActive);
            var query = custom_query ?? _ctx.EAMIS_UNITOFMEASURE;
            return query.Where(predicate);
        }

        public async Task<EamisUnitofMeasureDTO> Update(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public Task<bool> ValidateExistingDescUpdate(string ShortDesc, string UomDesc)
        {
            return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == ShortDesc && x.UOM_DESCRIPTION == UomDesc);
        }

        //public Task<bool> ValidateExistingUomDesc(string UomDesc)
        //{
        //    return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.UOM_DESCRIPTION != UomDesc);
        //}
        //public Task<bool> ValidateExistingUomDesc1(string UomDesc)
        //{
        //    return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.UOM_DESCRIPTION == UomDesc);
        //}

        //public Task<bool> ValidateExistingShortDesc1(string ShortDesc)
        //{
        //    return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == ShortDesc);
        //}
        //public Task<bool> ValidateExistingShortDesc(string ShortDesc)
        //{
        //    return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION != ShortDesc);
        //}
        //public Task<bool> Validation(string ShortDesc, string UomDesc)
        //{
        //    return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == ShortDesc && x.UOM_DESCRIPTION == UomDesc);

        //}
        public Task<bool> ValidationForUomExistShortDescNotExist (string ShortDesc,string UomDesc)
        {
            return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION != ShortDesc && x.UOM_DESCRIPTION == UomDesc);
        }
        public Task<bool> ValidationForShortDescExistUomNotExist (string ShortDesc, string UomDesc)
        {
            return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == ShortDesc && x.UOM_DESCRIPTION != UomDesc);
        }
        public Task<bool> ValidateExistDesc (string ShortDesc, string UomDesc)
        {
            return _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == ShortDesc && x.UOM_DESCRIPTION == UomDesc);
        }
        public async Task<bool> UpdateUOM(int id, string ShortDesc, string UomDesc)
        {
            return await _ctx.EAMIS_UNITOFMEASURE.AsNoTracking().AnyAsync(x => x.ID == id && x.SHORT_DESCRIPTION == ShortDesc && x.UOM_DESCRIPTION == UomDesc);
        }
    }
}
