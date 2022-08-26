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
        private string issuanceString()
        {
            return "anyTask";
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
            predicate = predicate.And(x => x.IS_ACTIVE == true);
            predicate = predicate.And(q => q.QUANTITY > 0);
            var query = custom_query ?? _ctx.EAMIS_PROPERTYITEMS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<EamisPropertyTransactionDetailsDTO> UpdateDetails(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = data.ID == 0 ? EntityState.Added : EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
     
        public async Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisPropertyTransactionDTO
            {
                Id = result.ID,
                TransactionNumber = result.TRANSACTION_NUMBER,
                TransactionDate = result.TRANSACTION_DATE,
                FundSource = result.FUND_SOURCE,
                FiscalPeriod = result.FISCALPERIOD,
                TransactionType = result.TRANSACTION_TYPE,
                Memo = result.MEMO,
                ReceivedBy = result.RECEIVED_BY,
                ApprovedBy = result.APPROVED_BY,
                DeliveryDate = result.DELIVERY_DATE,
                UserStamp = result.USER_STAMP,
                TimeStamp = result.TIMESTAMP,
                TransactionStatus = result.TRANSACTION_STATUS,
                PropertyTransactionDetails = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Select(x => new EamisPropertyTransactionDetailsDTO
                {
                    Id = x.ID,
                    PropertyTransactionID = x.PROPERTY_TRANS_ID,
                    isDepreciation = x.IS_DEPRECIATION,
                    Dr = x.DR,
                    PropertyNumber = x.PROPERTY_NUMBER,
                    ItemCode = x.ITEM_CODE,
                    ItemDescription = x.ITEM_DESCRIPTION,
                    SerialNumber = x.SERIAL_NUMBER,
                    Po = x.PO,
                    Pr = x.PR,
                    AcquisitionDate = x.ACQUISITION_DATE,
                    AssigneeCustodian = x.ASSIGNEE_CUSTODIAN,
                    RequestedBy = x.REQUESTED_BY,
                    Office = x.OFFICE,
                    Department = x.DEPARTMENT,
                    ResponsibilityCode = x.RESPONSIBILITY_CODE,
                    UnitCost = x.UNIT_COST,
                    Qty = x.QTY,
                    SalvageValue = x.SALVAGE_VALUE,
                    BookValue = x.BOOK_VALUE,
                    EstLife = x.ESTIMATED_LIFE,
                    Area = x.AREA,
                    Semi = x.SEMI_EXPANDABLE_AMOUNT,
                    UserStamp = x.USER_STAMP,
                    TimeStamp = x.TIME_STAMP,
                    WarrantyExpiry = x.WARRANTY_EXPIRY,
                    Invoice = x.INVOICE,
                    PropertyCondition = x.PROPERTY_CONDITION
                }).Where(i => i.PropertyTransactionID == result.ID).ToList()
            };
        }
        

        public async Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            //ensure that recently added record has the correct transaction id number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.ISPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

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
                FUND_SOURCE = item.FundSource,
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
                ITEM_CODE = item.ItemCode,
                ITEM_DESCRIPTION = item.ItemDescription,
                SERIAL_NUMBER = item.SerialNumber,
                PO = item.Po,
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
        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> SearchReceiving(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = null;
            if (type == "Item Code")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == "Property Receiving" && x.ITEM_CODE.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Item Description")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == "Property Receiving" && x.ITEM_DESCRIPTION.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Transaction Number")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == "Property Receiving" && x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == "Property Receiving" &&  x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryForSearch(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query;
        }

        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredItemsForReceiving(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQueryItemsForReceiving(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
            };
        }
        private IQueryable<EamisPropertyTransactionDetailsDTO> QueryToDTOItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransactionDetailsDTO
            {
                Id = x.ID,
                PropertyTransactionID = x.PROPERTY_TRANS_ID,
                isDepreciation = x.IS_DEPRECIATION,
                ItemCode = x.ITEM_CODE,
                Dr = x.DR,
                PropertyNumber = x.PROPERTY_NUMBER,
                ItemDescription = x.ITEM_DESCRIPTION,
                SerialNumber = x.SERIAL_NUMBER,
                Po = x.PO,
                Pr = x.PR,
                AcquisitionDate = x.ACQUISITION_DATE,
                AssigneeCustodian = x.ASSIGNEE_CUSTODIAN,
                RequestedBy = x.REQUESTED_BY,
                Office = x.OFFICE,
                Department = x.DEPARTMENT,
                ResponsibilityCode = x.RESPONSIBILITY_CODE,
                UnitCost = x.UNIT_COST,
                Qty = x.QTY,
                SalvageValue = x.SALVAGE_VALUE,
                BookValue = x.BOOK_VALUE,
                EstLife = x.ESTIMATED_LIFE,
                Area = x.AREA,
                Semi = x.SEMI_EXPANDABLE_AMOUNT,
                WarrantyExpiry = x.WARRANTY_EXPIRY,
                UserStamp = x.USER_STAMP,
                TimeStamp = x.TIME_STAMP,
                Invoice = x.INVOICE,
                PropertyCondition = x.PROPERTY_CONDITION,
                PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(x => new EamisPropertyTransactionDTO
                {
                    Id = x.ID,
                    TransactionStatus = x.TRANSACTION_STATUS,
                    TransactionType = x.TRANSACTION_TYPE,
                    TransactionNumber = x.TRANSACTION_NUMBER,
                    TransactionDate = x.TRANSACTION_DATE,
                    FundSource = x.FUND_SOURCE,
                    FiscalPeriod = x.FISCALPERIOD,
                    ReceivedBy = x.RECEIVED_BY
                }).Where(i => i.Id == x.PROPERTY_TRANS_ID).FirstOrDefault()

            }
            );
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            //Do not display items under service logs
            var arrservicelogs = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking()
                                .Where(pn => !(pn.PROPERTY_NUMBER == null || pn.PROPERTY_NUMBER.Trim() == string.Empty))
                                .Select(x => x.PROPERTY_NUMBER)
                                .ToList();
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Where(x => x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving)
                                        .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = x.d.ID,
                                            PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = x.d.IS_DEPRECIATION,
                                            DR = x.d.DR,
                                            PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
                                            ITEM_CODE = x.d.ITEM_CODE,
                                            ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
                                            SERIAL_NUMBER = x.d.SERIAL_NUMBER,
                                            PO = x.d.PO,
                                            PR = x.d.PR,
                                            ACQUISITION_DATE = x.d.ACQUISITION_DATE,
                                            ASSIGNEE_CUSTODIAN = x.d.ASSIGNEE_CUSTODIAN,
                                            REQUESTED_BY = x.d.REQUESTED_BY,
                                            OFFICE = x.d.OFFICE,
                                            DEPARTMENT = x.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = x.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = x.d.UNIT_COST,
                                            QTY = x.d.QTY,
                                            SALVAGE_VALUE = x.d.SALVAGE_VALUE,
                                            BOOK_VALUE = x.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = x.d.ESTIMATED_LIFE,
                                            AREA = x.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = x.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = x.d.USER_STAMP,
                                            TIME_STAMP = x.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = x.d.WARRANTY_EXPIRY,
                                            INVOICE = x.d.INVOICE,
                                            PROPERTY_CONDITION = x.d.PROPERTY_CONDITION
                                        }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER));
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<string> GetResponsibilityCenterByID(string responsibilityCode)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.Where(s => s.RESPONSIBILITY_CENTER == responsibilityCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].OFFICE_DESC.ToString() + ":" +
                           result[0].UNIT_DESC.ToString() + ":" + 
                           result[0].RESPONSIBILITY_CENTER.ToString();
            }
            return retValue;
        }
        public async Task<string> GetPropertyNumber(DateTime acquisitionDate, string responsibilityCode)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.Where(s => s.RESPONSIBILITY_CENTER == responsibilityCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            { 
                retValue = acquisitionDate.Year.ToString() +"-"+
                    result[0].SUB_GROUP_CODE.ToString() +"-"+
                    result[0].OFFICE_DESC;
            }
            return retValue;
        }
        //public async Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item)
        //{
        //    EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
        //    _ctx.Entry(data).State = EntityState.Deleted;
        //    await _ctx.SaveChangesAsync();
        //    return item;
        //}
    }
}