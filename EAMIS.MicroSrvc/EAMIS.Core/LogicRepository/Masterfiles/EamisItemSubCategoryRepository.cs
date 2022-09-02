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
    public class EamisItemSubCategoryRepository : IEamisItemSubCategoryRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisItemSubCategoryRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISITEMSUBCATEGORY>> ListAllItemSubCategories()
        {
            //IQueryable<EAMISITEMSUBCATEGORY> query = _ctx.EAMIS_ITEMS_SUB_CATEGORY;
            //var result = query.ToListAsync().GetAwaiter().GetResult();
            var result = _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().ToList();
            return result;
        }


        public async Task<bool> InsertFromExcel(List<EamisItemSubCategoryDTO> Items)
        {
            List<EAMISITEMSUBCATEGORY> lstSubCategory = new List<EAMISITEMSUBCATEGORY>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISITEMSUBCATEGORY objSubCategory = MapToEntity(Items[intItems]);

                    lstSubCategory.Add(objSubCategory);
                }
                _ctx.EAMIS_ITEMS_SUB_CATEGORY.AddRange(lstSubCategory);
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

        public async Task<EamisItemSubCategoryDTO> InsertFromExcel(EamisItemSubCategoryDTO item)
        {
            try
            {
                EAMISITEMSUBCATEGORY data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return item;
        }
        public async Task<EamisItemSubCategoryDTO> Delete(EamisItemSubCategoryDTO item)
        {
            EAMISITEMSUBCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISITEMSUBCATEGORY MapToEntity(EamisItemSubCategoryDTO item)
        {
            if (item == null) return new EAMISITEMSUBCATEGORY();
            return new EAMISITEMSUBCATEGORY
            {
                ID = item.Id,
                CATEGORY_ID = item.CategoryId,
                SUB_CATEGORY_NAME = item.SubCategoryName,
                IS_ACTIVE= item.IsActive
            };
        }

        public async Task<EamisItemSubCategoryDTO> Insert(EamisItemSubCategoryDTO item)
        {
            EAMISITEMSUBCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisItemSubCategoryDTO>> List(EamisItemSubCategoryDTO filter, PageConfig config)
        {
            IQueryable<EAMISITEMSUBCATEGORY> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisItemSubCategoryDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).OrderByDescending(x => x.Id).ToListAsync(),
            };
        }

        private IQueryable<EamisItemSubCategoryDTO> QueryToDTO(IQueryable<EAMISITEMSUBCATEGORY> query)
        {

            return query.Select(x => new EamisItemSubCategoryDTO
            {
                Id = x.ID,
                CategoryId = x.CATEGORY_ID,
                SubCategoryName = x.SUB_CATEGORY_NAME,
                IsActive = x.IS_ACTIVE,
                ItemCategory = new EamisItemCategoryDTO
                {
                    Id = x.ITEM_CATEGORY.ID,
                    CategoryName = x.ITEM_CATEGORY.CATEGORY_NAME
                }

                //ItemCategory = x.ITEM_CATEGORY.
               
            }) ;
        }

        private IQueryable<EAMISITEMSUBCATEGORY> PagedQuery(IQueryable<EAMISITEMSUBCATEGORY> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISITEMSUBCATEGORY> FilteredEntities(EamisItemSubCategoryDTO filter, IQueryable<EAMISITEMSUBCATEGORY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISITEMSUBCATEGORY>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (filter.CategoryId != null && filter.CategoryId != 0)
                predicate = predicate.And(x => x.CATEGORY_ID == filter.CategoryId);

            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);

            if (!string.IsNullOrEmpty(filter.SubCategoryName)) predicate = (strict)
                    ? predicate.And(x => x.SUB_CATEGORY_NAME.ToLower() == filter.SubCategoryName.ToLower())
                    : predicate.And(x => x.SUB_CATEGORY_NAME.ToLower() == filter.SubCategoryName.ToLower());

            var query = custom_query ?? _ctx.EAMIS_ITEMS_SUB_CATEGORY;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisItemSubCategoryDTO>> SearchItemSubCategory(string type, string searchValue)
        {
            IQueryable<EAMISITEMSUBCATEGORY> query = null;
            if (type == "Category Name")
            {
                query = _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().Where(x => x.ITEM_CATEGORY.CATEGORY_NAME.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().Where(x => x.SUB_CATEGORY_NAME.Contains(searchValue)).AsQueryable();
            }
            
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisItemSubCategoryDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISITEMSUBCATEGORY> PagedQueryForSearch(IQueryable<EAMISITEMSUBCATEGORY> query)
        {
            return query;
        }

        public async Task<EamisItemSubCategoryDTO> Update(EamisItemSubCategoryDTO item)
        {
            EAMISITEMSUBCATEGORY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public Task<bool> ValidateExistingSubUpdate(string SubCategoryName, int CategoryId)
        {
            return _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().AnyAsync(x => x.SUB_CATEGORY_NAME == SubCategoryName && x.CATEGORY_ID == CategoryId);
        }

        public Task<bool> ValidateExistingSub(int categoryId)
        {
            return _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().AnyAsync(x => x.CATEGORY_ID != categoryId);
        }

        public Task<bool> ValidateExistingCategoryId(int categoryId)
        {
            return _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().AnyAsync(x => x.CATEGORY_ID == categoryId);
        }

        public Task<bool> Validation(int categoryId, string subCategoryName)
        {
            return _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking().AnyAsync(x => x.CATEGORY_ID == categoryId && x.SUB_CATEGORY_NAME == subCategoryName);
           
        }
    }
}
