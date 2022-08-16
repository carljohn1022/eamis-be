using EAMIS.Common.DTO.Ais;
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
    public class EamisItemCategoryRepository : IEamisItemCategoryRepository
    {
        private readonly EAMISContext _ctx;
        private readonly AISContext _aisctx;
        private readonly int _maxPageSize;
        public EamisItemCategoryRepository(EAMISContext ctx, AISContext aisctx)
        {
            _ctx = ctx;
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISITEMCATEGORY>> ListAllItemCategories()
        {
            //IQueryable<EAMISITEMCATEGORY> query = _ctx.EAMIS_ITEM_CATEGORY;
            //var result =  query.ToListAsync().GetAwaiter().GetResult();
            var result = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToList();
            return result;
        }
        public async Task<List<EAMISITEMCATEGORY>> ListCategories(string searchValue)
        {
            var result = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Where(x => x.CATEGORY_NAME == searchValue).ToList();
            return result;
        }
        public async Task<List<EAMISITEMCATEGORY>> ListCategoriesById(int categoryId)
        {
            var result = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Where(x => x.ID == categoryId).ToList();
            return result;
        }
        public async Task<List<EAMISITEMCATEGORY>> GetAllItemCategories()
        {
            var result = await Task.Run(() => _ctx.EAMIS_ITEM_CATEGORY.ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<EamisItemCategoryDTO> InsertFromExcel(EamisItemCategoryDTO item)
        {
            try
            {
                EAMISITEMCATEGORY data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return item;
        }
        public async Task<EamisItemCategoryDTO> Delete(EamisItemCategoryDTO item)
        {
            EAMISITEMCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISITEMCATEGORY MapToEntity(EamisItemCategoryDTO item)
        {
            var chartAccount = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().ToList();

            if (item == null) return new EAMISITEMCATEGORY();
            return new EAMISITEMCATEGORY
            {
                ID = item.Id,
                SHORT_DESCRIPTION = item.ShortDesc,
                CHART_OF_ACCOUNT_ID = item.ChartOfAccountId,
                CATEGORY_NAME = item.CategoryName,
                COST_METHOD = item.CostMethod,
                DEPRECIATION_METHOD = item.DepreciationMethod,
                ESTIMATED_LIFE = item.EstimatedLife,
                IS_SERIALIZED = item.IsSerialized,
                IS_STOCKABLE = item.IsStockable,
                STOCK_QUANTITY = item.StockQuantity,
                IS_ASSET = item.IsAsset,
                IS_SUPPLIES = item.IsSupplies,
                IS_ACTIVE = item.IsActive
            };
        }

        public async Task<EamisItemCategoryDTO> Insert(EamisItemCategoryDTO item)
        {
            EAMISITEMCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisItemCategoryDTO>> List(EamisItemCategoryDTO filter, PageConfig config)
        {
            IQueryable<EAMISITEMCATEGORY> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisItemCategoryDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        //public async Task<DataList<EamisItemCategoryDTO>> MapToDTOList(string searchKey, PageConfig config)
        //{
        //    List<EAMISITEMCATEGORY> eamisItemCategory = new List<EAMISITEMCATEGORY>();
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        eamisItemCategory = await _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Where(x => x.CATEGORY_NAME.Contains(searchKey)).ToListAsync();
        //    }
        //    else
        //    {
        //        eamisItemCategory = await _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToListAsync();
        //    }
            
        //    string resolved_sort = config.SortBy ?? "Id";
        //    bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
        //    int resolved_size = config.Size ?? _maxPageSize;
        //    if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
        //    int resolved_index = config.Index ?? 1;
        //    var paged = PagedQueryMap(eamisItemCategory, resolved_size, resolved_index);
        //    return new DataList<EamisItemCategoryDTO>
        //    {
        //        Count = eamisItemCategory.Count(),
        //        Items = paged.Select(x => MapToDTO(x)).ToList()
        //    };
        //}

        private IQueryable<EAMISITEMCATEGORY> PagedQuery(IQueryable<EAMISITEMCATEGORY> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisItemCategoryDTO> QueryToDTO(IQueryable<EAMISITEMCATEGORY> query)
        {
            var aisOffice = _aisctx.Office.AsNoTracking().ToList();

            return query.Select(x => new EamisItemCategoryDTO
            {
                Id = x.ID,
                ShortDesc = x.SHORT_DESCRIPTION,
                ChartOfAccountId = x.CHART_OF_ACCOUNT_ID,
                CategoryName = x.CATEGORY_NAME,
                CostMethod = x.COST_METHOD,
                DepreciationMethod = x.DEPRECIATION_METHOD,
                EstimatedLife = x.ESTIMATED_LIFE,
                IsSerialized = x.IS_SERIALIZED,
                IsStockable = x.IS_STOCKABLE,
                StockQuantity = x.STOCK_QUANTITY,
                IsAsset = x.IS_ASSET,
                IsSupplies = x.IS_SUPPLIES,
                IsActive = x.IS_ACTIVE,
                ChartOfAccounts = new EamisChartofAccountsDTO
                {
                    Id = x.CHART_OF_ACCOUNTS.ID,
                    AccountCode = x.CHART_OF_ACCOUNTS.ACCOUNT_CODE,
                    ObjectCode = x.CHART_OF_ACCOUNTS.OBJECT_CODE
                },
                
            });
        }

        //private EamisItemCategoryDTO MapToDTO(EAMISITEMCATEGORY item)
        //{
        //    if (item == null) return new EamisItemCategoryDTO();
        //    var aisOffice = _aisctx.Office.AsNoTracking().ToList();
        //    var chartOfAccounts = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().ToList();

        //    return new EamisItemCategoryDTO
        //    {
        //        Id = item.ID,
        //        ChartOfAccountId = item.CHART_OF_ACCOUNT_ID,
        //        CategoryName = item.CATEGORY_NAME,
        //        CostMethod = item.COST_METHOD,
        //        DepreciationMethod = item.DEPRECIATION_METHOD,
        //        EstimatedLife = item.ESTIMATED_LIFE,
        //        IsSerialized = item.IS_SERIALIZED,
        //        IsStockable = item.IS_STOCKABLE,
        //        StockQuantity = item.STOCK_QUANTITY,
        //        ChartOfAccounts = new EamisChartofAccountsDTO
        //        {
        //            Id = chartOfAccounts.FirstOrDefault(x => x.ID == item.CHART_OF_ACCOUNT_ID).ID,
        //            AccountCode = chartOfAccounts.FirstOrDefault(x => x.ID == item.CHART_OF_ACCOUNT_ID).ACCOUNT_CODE,
        //            ObjectCode = chartOfAccounts.FirstOrDefault(x => x.ID == item.CHART_OF_ACCOUNT_ID).OBJECT_CODE,
        //        },
        //        OfficeInfo = new AisOfficeDTO
        //        {
        //            Id = aisOffice.FirstOrDefault(x => x.Id == item.OFFIICE_ID).Id,
        //            ShortName = aisOffice.FirstOrDefault(x => x.Id == item.OFFIICE_ID).ShortName,
        //        }
        //    };
        //}

        private IQueryable<EAMISITEMCATEGORY> FilteredEntities(EamisItemCategoryDTO filter, IQueryable<EAMISITEMCATEGORY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISITEMCATEGORY>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (filter.ChartOfAccountId != null && filter.ChartOfAccountId != 0)
                predicate = predicate.And(x => x.CHART_OF_ACCOUNT_ID == filter.ChartOfAccountId);

            //if (filter.OfficeId != null && filter.OfficeId != 0)
            //    predicate = predicate.And(x => x.OFFIICE_ID == filter.OfficeId);

            if (filter.StockQuantity != null && filter.StockQuantity != 0)
                predicate = predicate.And(x => x.STOCK_QUANTITY == filter.StockQuantity);

            if (filter.EstimatedLife != null && filter.EstimatedLife != 0)
                predicate = predicate.And(x => x.ESTIMATED_LIFE == filter.EstimatedLife);

            //if (!string.IsNullOrEmpty(filter.ResponsibilityCode)) predicate = (strict)
            //        ? predicate.And(x => x.RESPONSIBILITY_CODE.ToLower() == filter.ResponsibilityCode.ToLower())
            //        : predicate.And(x => x.RESPONSIBILITY_CODE.ToLower() == filter.ResponsibilityCode.ToLower());

            if (!string.IsNullOrEmpty(filter.DepreciationMethod)) predicate = (strict)
                    ? predicate.And(x => x.DEPRECIATION_METHOD.ToLower() == filter.DepreciationMethod.ToLower())
                    : predicate.And(x => x.DEPRECIATION_METHOD.ToLower() == filter.DepreciationMethod.ToLower());

            if (!string.IsNullOrEmpty(filter.CostMethod)) predicate = (strict)
                    ? predicate.And(x => x.COST_METHOD.ToLower() == filter.CostMethod.ToLower())
                    : predicate.And(x => x.COST_METHOD.ToLower() == filter.CostMethod.ToLower());

            if (!string.IsNullOrEmpty(filter.CategoryName)) predicate = (strict)
                    ? predicate.And(x => x.CATEGORY_NAME.ToLower() == filter.CategoryName.ToLower())
                    : predicate.And(x => x.CATEGORY_NAME.ToLower() == filter.CategoryName.ToLower());

            if (filter.IsStockable != null && filter.IsStockable != false)
                predicate = predicate.And(x => x.IS_STOCKABLE == filter.IsStockable);

            if (filter.IsSerialized != null && filter.IsSerialized != false)
                predicate = predicate.And(x => x.IS_SERIALIZED == filter.IsSerialized);

            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);

            var query = custom_query ?? _ctx.EAMIS_ITEM_CATEGORY;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisItemCategoryDTO>> SearchItemCategory(string type, string searchValue)
        {
            IQueryable<EAMISITEMCATEGORY> query = null;

            if(type == "Category Name")
            {
                query = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Where(x => x.CATEGORY_NAME.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Where(x => x.CHART_OF_ACCOUNTS.ACCOUNT_CODE.Contains(searchValue)).AsQueryable();
            }
 
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisItemCategoryDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISITEMCATEGORY> PagedQueryForSearch(IQueryable<EAMISITEMCATEGORY> query)
        {
            return query;
        }

        public List<EAMISITEMCATEGORY> PagedQueryMap(List<EAMISITEMCATEGORY> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size).ToList();
        }

        private object MapToDTO(object paged)
        {
            throw new NotImplementedException();
        }

        public async Task<EamisItemCategoryDTO> Update(EamisItemCategoryDTO item)
        {
            EAMISITEMCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public Task<bool> ValidateExistingShortDesc(string shortDesc , string categoryName)
        {
            return _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().AnyAsync(x => x.SHORT_DESCRIPTION == shortDesc || x.CATEGORY_NAME == categoryName);
        }

        public Task<bool> EditValidateExistingShortDesc(int id, string shortDesc, string categoryName)
        {
            return _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().AnyAsync(x => x.ID == id && x.SHORT_DESCRIPTION == shortDesc || x.CATEGORY_NAME == categoryName);
        }
        public async Task<string> GetPropertyNo(int categoryId)
        {
            string retValue = "";
            var maxId = 0;
            var nextId = 0;
            maxId = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.Max(t => (int?) t.ID) ?? 0).ConfigureAwait(false);
            nextId = maxId + 1;
            var result = await Task.Run(() => _ctx.EAMIS_ITEM_CATEGORY.Where(s => s.ID == categoryId).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].SHORT_DESCRIPTION.ToString() + nextId.ToString().PadLeft(6, '0');
            }
            return retValue;
        }
    }
}
