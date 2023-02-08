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
    public class EamisPropertyTransactionRepository : IEamisPropertyTransactionRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        public EamisPropertyTransactionRepository(EAMISContext ctx, IEAMISIDProvider EAMISIDProvider)
        {
            _EAMISIDProvider = EAMISIDProvider;
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<EamisPropertyTransactionDTO>> SearchReceivingforIssuance(string type, string searchValue, bool isProperty)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = null;
            if (type == "Transaction #")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.IS_PROPERTY == isProperty && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Received by")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.IS_PROPERTY == isProperty && x.RECEIVED_BY.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.IS_PROPERTY == isProperty && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyTransactionDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        public async Task<DataList<EamisPropertyTransactionDTO>> SearchReceivingforTransfer(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = null;
            if (type == "Transaction #")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Received by")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer && x.RECEIVED_BY.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyTransactionDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        public async Task<DataList<EamisPropertyTransactionDTO>> SearchReceivingforList(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = null;
            if (type == "Transaction #")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == "Property Receiving" && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Received By")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == "Property Receiving" && x.RECEIVED_BY.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == "Property Receiving" && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyTransactionDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISPROPERTYTRANSACTION> PagedQueryForSearch(IQueryable<EAMISPROPERTYTRANSACTION> query)
        {
            return query;
        }
        public async Task<EamisPropertyTransactionDTO> Delete(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPROPERTYTRANSACTION MapToEntity(EamisPropertyTransactionDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTION();
            return new EAMISPROPERTYTRANSACTION
            {
                ID = item.Id,
                TRANSACTION_NUMBER = item.TransactionNumber,
                TRAN_TYPE = item.TranType,
                TRANSACTION_DATE = item.TransactionDate,
                FISCALPERIOD = item.FiscalPeriod,
                FUND_SOURCE = item.FundSource,
                TRANSACTION_TYPE = item.TransactionType,
                MEMO = item.Memo,
                RECEIVED_BY = item.ReceivedBy,
                APPROVED_BY = item.ApprovedBy,
                DELIVERY_DATE = item.DeliveryDate,
                USER_STAMP = item.UserStamp,
                TIMESTAMP = item.TimeStamp,
                TRANSACTION_STATUS = item.TransactionStatus,
                IS_PROPERTY = item.IsProperty

            };
        }

        public async Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            //ensure that recently added record has the correct transaction type number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _prType = PrefixSettings.PRPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

            //check if the forecasted transaction type matches with the actual transaction type (saved/created in DB)
            //forecasted transaction type = item.TransactionType
            //actual transaction type = item.TransactionType.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0')
            if (item.TransactionNumber != _prType)
            {
                item.TransactionNumber = _prType; //if not matched, replace value of FTT with  ATT

                //reset context state to avoid error
                _ctx.Entry(data).State = EntityState.Detached;

                //call the update method, force to update the transaction type in the DB
                await this.Update(item);
            }

            return item;
        }

        public async Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyTransactionDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisPropertyTransactionDTO> QueryToDTO(IQueryable<EAMISPROPERTYTRANSACTION> query)
        {
            return query.Select(x => new EamisPropertyTransactionDTO
            {
                Id = x.ID,
                TransactionNumber = x.TRANSACTION_NUMBER,
                TransactionDate = x.TRANSACTION_DATE,
                FiscalPeriod = x.FISCALPERIOD,
                TransactionType = x.TRANSACTION_TYPE,
                Memo = x.MEMO,
                ReceivedBy = x.RECEIVED_BY,
                ApprovedBy = x.APPROVED_BY,
                DeliveryDate = x.DELIVERY_DATE,
                UserStamp = x.USER_STAMP,
                TimeStamp = x.TIMESTAMP,
                TransactionStatus = x.TRANSACTION_STATUS,
                FundSource = x.FUND_SOURCE //added for Report
                ,
                DeliveryImages = _ctx.EAMIS_ATTACHED_FILES.AsNoTracking().Select(v => new EamisAttachedFilesDTO
                {
                    Id = v.ID,
                    FileName = v.ATTACHED_FILENAME,
                    ModuleName = v.MODULE_NAME,
                    TransactionNumber = v.TRANID
                }).Where(i => i.TransactionNumber == x.TRANSACTION_NUMBER).ToList()
            });
        }

        private IQueryable<EAMISPROPERTYTRANSACTION> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTION> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISPROPERTYTRANSACTION> FilteredEntities(EamisPropertyTransactionDTO filter, IQueryable<EAMISPROPERTYTRANSACTION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTION>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.TransactionNumber)) predicate = (strict)
                     ? predicate.And(x => x.TRANSACTION_NUMBER.ToLower() == filter.TransactionNumber.ToLower())
                     : predicate.And(x => x.TRANSACTION_NUMBER.Contains(filter.TransactionNumber.ToLower()));
            if (filter.TransactionDate != null && filter.TransactionDate != DateTime.MinValue)
                predicate = predicate.And(x => x.TRANSACTION_DATE == filter.TransactionDate);
            if (!string.IsNullOrEmpty(filter.FiscalPeriod)) predicate = (strict)
                    ? predicate.And(x => x.FISCALPERIOD.ToLower() == filter.FiscalPeriod.ToLower())
                    : predicate.And(x => x.FISCALPERIOD.Contains(filter.FiscalPeriod.ToLower()));
            predicate = predicate.And(x => x.TRANSACTION_TYPE.Contains(filter.TransactionType.ToLower()));
            if (!string.IsNullOrEmpty(filter.Memo)) predicate = (strict)
                    ? predicate.And(x => x.MEMO.ToLower() == filter.Memo.ToLower())
                    : predicate.And(x => x.MEMO.Contains(filter.Memo.ToLower()));
            if (!string.IsNullOrEmpty(filter.ReceivedBy)) predicate = (strict)
                   ? predicate.And(x => x.RECEIVED_BY.ToLower() == filter.ReceivedBy.ToLower())
                   : predicate.And(x => x.RECEIVED_BY.Contains(filter.ReceivedBy.ToLower()));
            if (!string.IsNullOrEmpty(filter.ApprovedBy)) predicate = (strict)
                   ? predicate.And(x => x.APPROVED_BY.ToLower() == filter.ApprovedBy.ToLower())
                   : predicate.And(x => x.APPROVED_BY.Contains(filter.ApprovedBy.ToLower()));
            if (filter.DeliveryDate != null && filter.DeliveryDate != DateTime.MinValue)
                predicate = predicate.And(x => x.DELIVERY_DATE == filter.DeliveryDate);
            if (!string.IsNullOrEmpty(filter.UserStamp)) predicate = (strict)
                   ? predicate.And(x => x.USER_STAMP.ToLower() == filter.UserStamp.ToLower())
                   : predicate.And(x => x.USER_STAMP.Contains(filter.UserStamp.ToLower()));
            if (!string.IsNullOrEmpty(filter.TimeStamp)) predicate = (strict)
                   ? predicate.And(x => x.TIMESTAMP.ToLower() == filter.TimeStamp.ToLower())
                   : predicate.And(x => x.TIMESTAMP.Contains(filter.TimeStamp.ToLower()));
            if (!string.IsNullOrEmpty(filter.TransactionStatus)) predicate = (strict)
                   ? predicate.And(x => x.TRANSACTION_STATUS.ToLower() == filter.TransactionStatus.ToLower())
                   : predicate.And(x => x.TRANSACTION_STATUS.Contains(filter.TransactionStatus.ToLower()));
            if (!string.IsNullOrEmpty(filter.FundSource)) predicate = (strict)
                    ? predicate.And(x => x.FUND_SOURCE.ToLower() == filter.FundSource.ToLower())
                    : predicate.And(x => x.FUND_SOURCE.Contains(filter.FundSource.ToLower()));
            if (filter.IsProperty != null)
                predicate = predicate.And(x => x.IS_PROPERTY == filter.IsProperty);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDTO> Update(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<string> GetNextSequenceNumberPR()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumberPR(TransactionTypeSettings.PropertyReceiving);
            return nextId;
        }
        public async Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisPropertyTransactionDTO
            {
                Id = result.ID,
                TransactionNumber = result.TRANSACTION_NUMBER,
                TransactionDate = result.TRANSACTION_DATE,
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
                ,
                DeliveryImages = _ctx.EAMIS_ATTACHED_FILES.AsNoTracking().Select(v => new EamisAttachedFilesDTO
                {
                    Id = v.ID,
                    FileName = v.ATTACHED_FILENAME,
                    ModuleName = v.MODULE_NAME,
                    TransactionNumber = v.TRANID
                }).Where(i => i.TransactionNumber == result.TRANSACTION_NUMBER).ToList()
            };
        }
        public async Task<DataList<EamisDeliveryReceiptDTO>> DeliveryReceiptHeaderToDetails(EamisDeliveryReceiptDTO filter, PageConfig config)
        {
            IQueryable<EAMISDELIVERYRECEIPT> query = DRHDFiltered(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = DRHDPageQuery(query, resolved_size, resolved_index);
            return new DataList<EamisDeliveryReceiptDTO>
            {
                Count = await query.CountAsync(),
                Items = await DRHDDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISDELIVERYRECEIPT> DRHDFiltered(EamisDeliveryReceiptDTO filter, IQueryable<EAMISDELIVERYRECEIPT> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISDELIVERYRECEIPT>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.TransactionType)) predicate = (strict)
                   ? predicate.And(x => x.TRANSACTION_TYPE.ToLower() == filter.TransactionType.ToLower())
                   : predicate.And(x => x.TRANSACTION_TYPE.Contains(filter.TransactionType.ToLower()));
            if (filter.SupplierId != null && filter.SupplierId != 0)
                predicate = predicate.And(x => x.SUPPLIER_ID == filter.SupplierId);
            if (!string.IsNullOrEmpty(filter.PurchaseOrderNumber)) predicate = (strict)
                  ? predicate.And(x => x.PURCHASE_ORDER_NUMBER.ToLower() == filter.PurchaseOrderNumber.ToLower())
                  : predicate.And(x => x.PURCHASE_ORDER_NUMBER.Contains(filter.PurchaseOrderNumber.ToLower()));
            if (filter.PurchaseOrderDate != null && filter.PurchaseOrderDate != DateTime.MinValue)
                predicate = predicate.And(x => x.PURCHASE_ORDER_DATE == filter.PurchaseOrderDate);
            if (!string.IsNullOrEmpty(filter.PurchaseRequestNumber)) predicate = (strict)
                  ? predicate.And(x => x.PURCHASE_REQUEST_NUMBER.ToLower() == filter.PurchaseRequestNumber.ToLower())
                  : predicate.And(x => x.PURCHASE_REQUEST_NUMBER.Contains(filter.PurchaseRequestNumber.ToLower()));
            if (filter.PurchaseRequestDate != null && filter.PurchaseRequestDate != DateTime.MinValue)
                predicate = predicate.And(x => x.PURCHASE_REQUEST_DATE == filter.PurchaseRequestDate);
            if (!string.IsNullOrEmpty(filter.SaleInvoiceNumber)) predicate = (strict)
                              ? predicate.And(x => x.SALE_INVOICE_NUMBER.ToLower() == filter.SaleInvoiceNumber.ToLower())
                              : predicate.And(x => x.SALE_INVOICE_NUMBER.Contains(filter.SaleInvoiceNumber.ToLower()));
            if (filter.SaleInvoiceDate != null && filter.SaleInvoiceDate != DateTime.MinValue)
                predicate = predicate.And(x => x.SALE_INVOICE_DATE == filter.SaleInvoiceDate);
            if (filter.TotalAmount != null && filter.TotalAmount != 0)
                predicate = predicate.And(x => x.TOTAL_AMOUNT == filter.TotalAmount);
            if (!string.IsNullOrEmpty(filter.TransactionStatus)) predicate = (strict)
                   ? predicate.And(x => x.TRANSACTION_STATUS.ToLower() == filter.TransactionStatus.ToLower())
                   : predicate.And(x => x.TRANSACTION_STATUS.Contains(filter.TransactionStatus.ToLower()));

            predicate = predicate.And(x => x.DELIVERY_RECEIPT_DETAILS.Where(y => y.ITEMS_GROUP.ITEM_CATEGORY.IS_ASSET == true).Any());

            var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT;
            return query.Where(predicate);
        }
        private IQueryable<EAMISDELIVERYRECEIPT> DRHDPageQuery(IQueryable<EAMISDELIVERYRECEIPT> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EamisDeliveryReceiptDTO> DRHDDTO(IQueryable<EAMISDELIVERYRECEIPT> query)
        {
            return query.Select(x => new EamisDeliveryReceiptDTO
            {
                Id = x.ID,
                TransactionType = x.TRANSACTION_TYPE,
                ReceivedBy = x.RECEIVED_BY,
                DateReceived = x.DATE_RECEIVED,
                SupplierId = x.SUPPLIER_ID,
                PurchaseOrderNumber = x.PURCHASE_ORDER_NUMBER,
                PurchaseOrderDate = x.PURCHASE_ORDER_DATE,
                PurchaseRequestNumber = x.PURCHASE_REQUEST_NUMBER,
                PurchaseRequestDate = x.PURCHASE_REQUEST_DATE,
                SaleInvoiceNumber = x.SALE_INVOICE_NUMBER,
                SaleInvoiceDate = x.SALE_INVOICE_DATE,
                TotalAmount = x.TOTAL_AMOUNT,
                TransactionStatus = x.TRANSACTION_STATUS,
                StockroomId = x.WAREHOUSE_ID,
                DRNumFrSupplier = x.DR_BY_SUPPLIER_NUMBER,
                DRDate = x.DR_BY_SUPPLIER_DATE,
                Warehouse = new EamisWarehouseDTO
                {
                    Id = x.WAREHOUSE_GROUP.ID,
                    Warehouse_Description = x.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION
                },
                Supplier = new EamisSupplierDTO
                {
                    Id = x.SUPPLIER_GROUP.ID,
                    CompanyName = x.SUPPLIER_GROUP.COMPANY_NAME
                },
                DeliveryImages = _ctx.EAMIS_ATTACHED_FILES.AsNoTracking().Select(v => new EamisAttachedFilesDTO
                {
                    Id = v.ID,
                    FileName = v.ATTACHED_FILENAME,
                    ModuleName = v.MODULE_NAME,
                    TransactionNumber = v.TRANID
                }).Where(i => i.TransactionNumber == x.TRANSACTION_TYPE).ToList(),

                DeliveryReceiptDetails = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Select(y => new EamisDeliveryReceiptDetailsDTO
                {
                    Id = y.ID,
                    DeliveryReceiptId = y.DELIVERY_RECEIPT_ID,
                    ItemId = y.ITEM_ID,
                    QtyOrder = y.QTY_ORDER,
                    QtyDelivered = y.QTY_DELIVERED,
                    QtyRejected = y.QTY_REJECTED,
                    QtyReceived = y.QTY_RECEIVED,
                    UnitCost = y.UNIT_COST,
                    SerialNumber = y.SERIAL_LOT,
                    UnitOfMeasurement = y.UNIT_OF_MEASUREMENT,
                    SubTotal = y.SUB_TOTAL,
                    PropertyItem = new EamisPropertyItemsDTO
                    {
                        Id = y.ITEMS_GROUP.ID,
                        PropertyNo = y.ITEMS_GROUP.PROPERTY_NO,
                        PropertyName = y.ITEMS_GROUP.PROPERTY_NAME,
                        UomId = y.ITEMS_GROUP.UOM_ID,
                        ItemCategory = new EamisItemCategoryDTO
                        {
                            Id = y.ITEMS_GROUP.ITEM_CATEGORY.ID,
                            ForDepreciation = y.ITEMS_GROUP.ITEM_CATEGORY.FOR_DEPRECIATION,
                            IsAsset = y.ITEMS_GROUP.ITEM_CATEGORY.IS_ASSET
                        }
                    },
                    PropertySerialTran = _ctx.EAMIS_SERIAL_TRAN.AsNoTracking().Select(s => new EamisSerialTranDTO
                    {
                        Id = s.ID,
                        SerialNumber = s.SERIAL_NO,
                        WarrantyExpiryDate = s.WARRANTY_EXPIRY_DATE,
                        DeliveryReceiptDetailsId = s.DELIVERY_RECEIPT_DETAILS_ID
                    }).Where(i => i.DeliveryReceiptDetailsId == y.ID).ToList()
                }).Where(i => i.DeliveryReceiptId == x.ID).ToList()
            });

        }
    }


}
