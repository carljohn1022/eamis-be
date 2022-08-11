using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyIssuanceRepository : IEamisPropertyIssuanceRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        private readonly IEamisPropertyTransactionDetailsRepository _eamisPropertyTransactionDetailsRepository;
        public EamisPropertyIssuanceRepository(EAMISContext ctx,
                                               IEAMISIDProvider EAMISIDProvider,
                                               IEamisPropertyTransactionDetailsRepository eamisPropertyTransactionDetailsRepository)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _eamisPropertyTransactionDetailsRepository = eamisPropertyTransactionDetailsRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<string> GetNextSequenceNumber()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumber(TransactionTypeSettings.Issuance);
            return nextId;
        }

        public async Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item)
        {
            string strResult = "";
            //check item in DB
            var itemInDB = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.FirstOrDefault(i => i.ID == item.ItemId)).ConfigureAwait(false);
            if (itemInDB != null)
            {
                //update item qty
                itemInDB.QUANTITY = itemInDB.QUANTITY + item.QtyReceived;
                var result = await _ctx.SaveChangesAsync();
                if (result > 0)
                {
                    strResult = "successfully updated.";
                }
            }
            return strResult;
        }

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
                    SubCategory = x.ITEM_CATEGORY.ITEM_SUB_CATEGORY.Select(y => new EamisItemSubCategoryDTO { Id = y.ID, CategoryId = y.CATEGORY_ID, SubCategoryName = y.SUB_CATEGORY_NAME }).ToList()
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
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISPROPERTYITEMS> FilteredEntites(EamisPropertyItemsDTO filter, IQueryable<EAMISPROPERTYITEMS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYITEMS>(true);
            //if (filter.Id != null && filter.Id != 0)
            //    predicate = predicate.And(x => x.ID == filter.Id);

            //if (string.IsNullOrEmpty(filter.AppNo) && filter.AppNo != null)
            //    predicate = predicate.And(x => x.APP_NO == filter.AppNo);

            //if (string.IsNullOrEmpty(filter.PropertyNo) && filter.PropertyNo != null)
            //    predicate = predicate.And(x => x.PROPERTY_NO == filter.PropertyNo);

            //if (string.IsNullOrEmpty(filter.PropertyName) && filter.PropertyName != null)
            //    predicate = predicate.And(x => x.PROPERTY_NAME == filter.PropertyName);

            //if (filter.CategoryId != null && filter.CategoryId != 0)
            //    predicate = predicate.And(x => x.CATEGORY_ID == filter.CategoryId);

            //if (string.IsNullOrEmpty(filter.Brand) && filter.Brand != null)
            //    predicate = predicate.And(x => x.BRAND == filter.Brand);

            //if (filter.UomId != null && filter.UomId != 0)
            //    predicate = predicate.And(x => x.UOM_ID == filter.UomId);

            //if (filter.WarehouseId != null && filter.WarehouseId != 0)
            //    predicate = predicate.And(x => x.WAREHOUSE_ID == filter.WarehouseId);

            //if (string.IsNullOrEmpty(filter.PropertyType) && filter.PropertyType != null)
            //    predicate = predicate.And(x => x.PROPERTY_TYPE == filter.PropertyType);

            //if (string.IsNullOrEmpty(filter.Model) && filter.Model != null)
            //    predicate = predicate.And(x => x.MODEL == filter.Model);

            //if (filter.Quantity != null && filter.Quantity != 0)
            //    predicate = predicate.And(x => x.QUANTITY == filter.Quantity);

            //if (filter.SupplierId != null && filter.SupplierId != 0)
            //    predicate = predicate.And(x => x.SUPPLIER_ID == filter.SupplierId);

            //if (filter.IsActive != null && filter.IsActive != false)
            predicate = predicate.And(x => x.IS_ACTIVE == true);
            predicate = predicate.And(q => q.QUANTITY > 0);

            var query = custom_query ?? _ctx.EAMIS_PROPERTYITEMS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item)
        {
            if (item.Id == 0)
                return item;
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            //ensure that recently added record has the correct transaction id number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.ISPrefix + Convert.ToString(data.ID).PadLeft(6, '0');

            if (item.TransactionNumber != _drType)
            {
                item.TransactionNumber = _drType;

                //reset context state to avoid error
                _ctx.Entry(data).State = EntityState.Detached;

                //call the update method, force to update the transaction number in DB
                await this.UpdateProperty(item);
            }
            item.Id = data.ID;
            return item;
        }
        private EAMISPROPERTYTRANSACTION MapToEntity(EamisPropertyTransactionDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTION();
            return new EAMISPROPERTYTRANSACTION
            {
                ID = item.Id,
                TRANSACTION_NUMBER = item.TransactionNumber,
                TRANSACTION_DATE = item.TransactionDate,
                FISCALPERIOD = item.FiscalPeriod,
                TRANSACTION_TYPE = item.TransactionType,
                MEMO = item.Memo,
                RECEIVED_BY = item.ReceivedBy,
                APPROVED_BY = item.ApprovedBy,
                DELIVERY_DATE = item.DeliveryDate,
                USER_STAMP = item.UserStamp,
                TIMESTAMP = item.TimeStamp,
                TRANSACTION_STATUS = item.TransactionStatus
            };
        }

        public async Task<EamisPropertyTransactionDetailsDTO> InsertPropertyTransaction(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }
        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTIONDETAILS();
            return new EAMISPROPERTYTRANSACTIONDETAILS
            {
                ID = item.Id,
                PROPERTY_TRANS_ID = item.PropertyTransactionID,
                IS_DEPRECIATION = item.isDepreciation,
                DR = item.Dr,
                PROPERTY_NUMBER = item.PropertyNumber,
                ITEM_DESCRIPTION = item.ItemDescription,
                SERIAL_NUMBER = item.SerialNumber,
                PO = item.Pr,
                PR = item.Pr,
                ACQUISITION_DATE = item.AcquisitionDate,
                ASSIGNEE_CUSTODIAN = item.AssigneeCustodian,
                REQUESTED_BY = item.RequestedBy,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                RESPONSIBILITY_CODE = item.ResponsibilityCode,
                UNIT_COST = item.UnitCost,
                QTY = item.Qty,
                SALVAGE_VALUE = item.SalvageValue,
                BOOK_VALUE = item.BookValue,
                ESTIMATED_LIFE = item.EstLife,
                AREA = item.Area,
                SEMI_EXPANDABLE_AMOUNT = item.Semi,
                USER_STAMP = item.UserStamp,
                TIME_STAMP = item.TimeStamp,
                WARRANTY_EXPIRY = item.WarrantyExpiry,
                INVOICE = item.Invoice,
                PROPERTY_CONDITION = item.PropertyCondition

            };
        }

    }
}
