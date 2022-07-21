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
    public class EamisPropertyItemsRepository : IEamisPropertyItemsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private List<EamisPropertyTypeDTO> dto = new List<EamisPropertyTypeDTO>()
        {
            new EamisPropertyTypeDTO{ Id = 1, Amount = 500,DescriptionRules ="Sample",Name = "PPE" },
            new EamisPropertyTypeDTO{ Id = 2, Amount = 15000 ,DescriptionRules ="Sample",Name = "PPE" },
            new EamisPropertyTypeDTO { Id = 1, Amount = 500,DescriptionRules ="Sample",Name = "PPE" }
        };
       

      

        public EamisPropertyItemsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisPropertyItemsDTO> InsertFromExcel(EamisPropertyItemsDTO item)
        {
            try
            {
                EAMISPROPERTYITEMS data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;

                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return item;
        }
        public async Task<EamisPropertyItemsDTO> Delete(EamisPropertyItemsDTO item)
        {
            EAMISPROPERTYITEMS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPROPERTYITEMS MapToEntity(EamisPropertyItemsDTO item)
        {
            var category = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToList();
            if (item == null) return new EAMISPROPERTYITEMS();
            return new EAMISPROPERTYITEMS
            {
                ID = item.Id,
                PROPERTY_NO = item.PropertyNo,
                APP_NO = item.AppNo,
                PROPERTY_NAME = item.PropertyName,
                CATEGORY_ID = item.CategoryId,
                SUBCATEGORY_ID = item.SubCategoryId,
                BRAND = item.Brand,
                UOM_ID = item.UomId,
                WAREHOUSE_ID = item.WarehouseId,
                PROPERTY_TYPE = item.PropertyType,
                MODEL = item.Model,
                QUANTITY = item.Quantity,
                SUPPLIER_ID = item.SupplierId,
                IS_ACTIVE = item.IsActive
                
            };
        }

        public async Task<EamisPropertyItemsDTO> Insert(EamisPropertyItemsDTO item)
        {
          
            EAMISPROPERTYITEMS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            item.Id = data.ID;
            string _PropertyNo = item.PropertyNo.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0');
            if(item.PropertyNo != _PropertyNo)
            {
                item.PropertyNo = _PropertyNo;
                _ctx.Entry(data).State = EntityState.Detached;
                await this.Update(item);
            }
            return item;
        }
        

        public async Task<DataList<EamisPropertyItemsDTO>> PublicSearch(string type,string SearchValue)
        {

            IQueryable<EAMISPROPERTYITEMS> query = null;
            if(type == "Item Number")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.PROPERTY_NO.Contains(SearchValue)).AsQueryable();
            }
            else if (type == "Item Name")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.PROPERTY_NAME.Contains(SearchValue)).AsQueryable();
            }
            else if (type == "Brand")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.BRAND.Contains(SearchValue)).AsQueryable();
            }
            else if (type == "Model")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.MODEL.Contains(SearchValue)).AsQueryable();
            }
            else if (type == "Item Code")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.PROPERTY_NO.Contains(SearchValue)).AsQueryable();
            }
            else if (type == "Description")
            {
                query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.PROPERTY_NAME.Contains(SearchValue)).AsQueryable();
            }
            
           
           
       
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyItemsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await MapToDTO(paged).ToListAsync()
            };

        }

        //public async Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config, string SearchType, string SearchValue)
        //{
        //    IQueryable<EAMISPROPERTYITEMS> query = FilteredEntites(filter);
        //    string resolved_sort = config.SortBy ?? "Id";
        //    bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
        //    int resolved_size = config.Size ?? _maxPageSize;
        //    if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
        //    int resolved_index = config.Index ?? 1;
        //    if (SearchType == "Property Item")
        //    {
        //        if (string.IsNullOrEmpty(SearchValue))
        //        {
        //            query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().AsQueryable();
        //        }
        //        else
        //        {
        //            query = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Where(x => x.PROPERTY_ITEM_NAME.Contains(SearchValue)).AsQueryable();
        //        }

        //    }

        //    var paged = PagedQuery(query, resolved_size, resolved_index);
        //    return new DataList<EamisPropertyItemsDTO>
        //    {
        //        Count = await query.CountAsync(),
        //        Items = await QueryToDTO(paged).ToListAsync()
        //    };

        //}

        public async Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYITEMS> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyItemsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EamisPropertyItemsDTO> MapToDTO(IQueryable<EAMISPROPERTYITEMS> query)
        {
            return query.Select(x => new EamisPropertyItemsDTO
            {
                Id = x.ID,
                AppNo = x.APP_NO,
                PropertyNo = x.PROPERTY_NO,
                PropertyName = x.PROPERTY_NAME,
                CategoryId = x.CATEGORY_ID,
                Brand = x.BRAND,
                UomId = x.UOM_ID,
                WarehouseId = x.WAREHOUSE_ID,
                PropertyType = x.PROPERTY_TYPE,
                Model = x.MODEL,
                Quantity = x.QUANTITY,
                SupplierId = x.SUPPLIER_ID,
                IsActive = x.IS_ACTIVE,
                ItemCategory = new EamisItemCategoryDTO
                {
                    Id = x.ITEM_CATEGORY.ID,
                    CategoryName = x.ITEM_CATEGORY.CATEGORY_NAME,
                    ShortDesc = x.ITEM_CATEGORY.SHORT_DESCRIPTION,
                    SubCategory = x.ITEM_CATEGORY.ITEM_SUB_CATEGORY.Select(y => new EamisItemSubCategoryDTO { Id = y.ID, CategoryId = y.CATEGORY_ID, SubCategoryName = y.SUB_CATEGORY_NAME }).ToList(),
                    IsSerialized = x.ITEM_CATEGORY.IS_SERIALIZED,
                },
                UnitOfMeasure = new EamisUnitofMeasureDTO
                {
                    Id = x.UOM_GROUP.ID,
                    Uom_Description = x.UOM_GROUP.UOM_DESCRIPTION
                },
                Warehouse = new EamisWarehouseDTO
                {
                    Id = x.WAREHOUSE_GROUP.ID,
                    Warehouse_Description = x.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION
                },
                Supplier = new EamisSupplierDTO
                {
                    Id = x.SUPPLIER_GROUP.ID,
                    CompanyName = x.SUPPLIER_GROUP.COMPANY_NAME
                }


            });
        }

        public IQueryable<EAMISPROPERTYITEMS> PagedQueryForSearch(IQueryable<EAMISPROPERTYITEMS> query)
        {
            return query;
        }

        private IQueryable<EamisPropertyItemsDTO> QueryToDTO(IQueryable<EAMISPROPERTYITEMS> query)
        {
            var category = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToList();

            return query.Select(x => new EamisPropertyItemsDTO
            {
                Id = x.ID,
                AppNo = x.APP_NO,
                PropertyNo = x.PROPERTY_NO,
                PropertyName = x.PROPERTY_NAME,
                CategoryId = x.CATEGORY_ID,
                SubCategoryId = x.SUBCATEGORY_ID,
                Brand = x.BRAND,
                UomId = x.UOM_ID,
                WarehouseId = x.WAREHOUSE_ID,
                PropertyType = x.PROPERTY_TYPE,
                Model = x.MODEL,
                Quantity = x.QUANTITY,
                SupplierId = x.SUPPLIER_ID,
                IsActive = x.IS_ACTIVE,
                ItemCategory = new EamisItemCategoryDTO
                {
                    Id = x.ITEM_CATEGORY.ID,
                    CategoryName = x.ITEM_CATEGORY.CATEGORY_NAME,
                    ShortDesc = x.ITEM_CATEGORY.SHORT_DESCRIPTION,
                    SubCategory = x.ITEM_CATEGORY.ITEM_SUB_CATEGORY.Select(y => new EamisItemSubCategoryDTO {Id = y.ID, CategoryId = y.CATEGORY_ID, SubCategoryName = y.SUB_CATEGORY_NAME}).ToList(),
                    IsSerialized = x.ITEM_CATEGORY.IS_SERIALIZED
                },
                UnitOfMeasure = new EamisUnitofMeasureDTO
                {
                    Id = x.UOM_GROUP.ID,
                    Uom_Description = x.UOM_GROUP.UOM_DESCRIPTION
                },
                Warehouse = new EamisWarehouseDTO
                {
                    Id = x.WAREHOUSE_GROUP.ID,
                    Warehouse_Description = x.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION
                },
                Supplier = new EamisSupplierDTO
                {
                    Id = x.SUPPLIER_GROUP.ID,
                    CompanyName = x.SUPPLIER_GROUP.COMPANY_NAME
                }


            });
        }

        private IQueryable<EAMISPROPERTYITEMS> PagedQuery(IQueryable<EAMISPROPERTYITEMS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x=>x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISPROPERTYITEMS> FilteredEntites(EamisPropertyItemsDTO filter, IQueryable<EAMISPROPERTYITEMS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYITEMS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (string.IsNullOrEmpty(filter.AppNo) && filter.AppNo != null)
                predicate = predicate.And(x => x.APP_NO == filter.AppNo);

            if (string.IsNullOrEmpty(filter.PropertyNo) && filter.PropertyNo != null)
                predicate = predicate.And(x => x.PROPERTY_NO == filter.PropertyNo);

            if (string.IsNullOrEmpty(filter.PropertyName) && filter.PropertyName != null)
                predicate = predicate.And(x => x.PROPERTY_NAME == filter.PropertyName);

            if (filter.CategoryId != null && filter.CategoryId != 0)
                predicate = predicate.And(x => x.CATEGORY_ID == filter.CategoryId);

            if (string.IsNullOrEmpty(filter.Brand) && filter.Brand != null)
                predicate = predicate.And(x => x.BRAND == filter.Brand);

            if (filter.UomId != null && filter.UomId != 0)
                predicate = predicate.And(x => x.UOM_ID == filter.UomId);

            if (filter.WarehouseId != null && filter.WarehouseId != 0)
                predicate = predicate.And(x => x.WAREHOUSE_ID == filter.WarehouseId);

            if (string.IsNullOrEmpty(filter.PropertyType) && filter.PropertyType != null)
                predicate = predicate.And(x => x.PROPERTY_TYPE == filter.PropertyType);

            if (string.IsNullOrEmpty(filter.Model) && filter.Model != null)
                predicate = predicate.And(x => x.MODEL == filter.Model);

            if (filter.Quantity != null && filter.Quantity != 0)
                predicate = predicate.And(x => x.QUANTITY == filter.Quantity);

            if (filter.SupplierId != null && filter.SupplierId != 0)
                predicate = predicate.And(x => x.SUPPLIER_ID == filter.SupplierId);

            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);

            var query = custom_query ?? _ctx.EAMIS_PROPERTYITEMS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyItemsDTO> Update(EamisPropertyItemsDTO item)
        {
            EAMISPROPERTYITEMS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisPropertyItemsDTO> GeneratedProperty()
        {
            var property = await _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().OrderByDescending(x => x.ID).ToListAsync();
            var specificProperty = property.FirstOrDefault();
            EamisPropertyItemsDTO propertyItems = new EamisPropertyItemsDTO
            {
                Id = specificProperty.ID,
                AppNo = specificProperty.APP_NO
            };
            return  propertyItems;
        }


        public Task<bool> ValidateExistingItem(string propertyNo)
        {
            return _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().AnyAsync(x => x.PROPERTY_NO == propertyNo);
        }

        public Task<bool> UpdateValidateExistingItem(string propertyNo,int id)
        {
            return _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().AnyAsync(x => x.PROPERTY_NO == propertyNo && x.ID == id);
        }
    }
}
