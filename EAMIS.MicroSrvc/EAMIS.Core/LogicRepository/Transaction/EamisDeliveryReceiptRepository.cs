using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;

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
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;
using System.Collections.Generic;
using EAMIS.Common.DTO.LookUp;

namespace EAMIS.Core.LogicRepository.Transaction
{
   public class EamisDeliveryReceiptRepository : IEamisDeliveryReceiptRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;

        public EamisDeliveryReceiptRepository(EAMISContext ctx, IEAMISIDProvider EAMISIDProvider)
        {
            _EAMISIDProvider = EAMISIDProvider;
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisDeliveryReceiptDTO> Delete(EamisDeliveryReceiptDTO item)
        {
            EAMISDELIVERYRECEIPT data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }
        private EAMISDELIVERYRECEIPT MapToEntity(EamisDeliveryReceiptDTO item)
        {
            if (item == null) return new EAMISDELIVERYRECEIPT();
            return new EAMISDELIVERYRECEIPT
            {
                ID = item.Id,
                TRANSACTION_TYPE = item.TransactionType,
                RECEIVED_BY = item.ReceivedBy,
                DATE_RECEIVED = item.DateReceived,
                SUPPLIER_ID = item.SupplierId,
                PURCHASE_ORDER_NUMBER = item.PurchaseOrderNumber,
                PURCHASE_ORDER_DATE = item.PurchaseOrderDate,
                PURCHASE_REQUEST_NUMBER = item.PurchaseRequestNumber,
                PURCHASE_REQUEST_DATE = item.PurchaseRequestDate,
                SALE_INVOICE_NUMBER = item.SaleInvoiceNumber,
                SALE_INVOICE_DATE = item.SaleInvoiceDate,
                TOTAL_AMOUNT = item.TotalAmount,
                TRANSACTION_STATUS = item.TransactionStatus,
                WAREHOUSE_ID = item.StockroomId,
                DR_BY_SUPPLIER_NUMBER = item.DRNumFrSupplier,
                DR_BY_SUPPLIER_DATE = item.DRDate,
                APR_NUMBER = item.AprNum,
                APR_DATE = item.AprDate,
                USER_STAMP = item.UserStamp
            };
        }
        //public async Task<EamisDeliveryReceiptDTO> GeneratedDRNum()
        //{
        //    var deliveryPrefix = "DR" + DateTime.Now.Year.ToString();
        //    var idNum = await _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().OrderByDescending(x => x.ID).ToListAsync();
        //    var drNumber = deliveryPrefix + idNum;
            
        //}

        public async Task<EamisDeliveryReceiptDTO> Insert(EamisDeliveryReceiptDTO item)
        {
            EAMISDELIVERYRECEIPT data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            //ensure that recently added record has the correct transaction type number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.DRPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

            //check if the forecasted transaction type matches with the actual transaction type (saved/created in DB)
            //forecasted transaction type = item.TransactionType
            //actual transaction type = item.TransactionType.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0')
            if (item.TransactionType != _drType)
            {
                item.TransactionType = _drType; //if not matched, replace value of FTT with  ATT

                //reset context state to avoid error
                _ctx.Entry(data).State = EntityState.Detached;

                //call the update method, force to update the transaction type in the DB
                await this.Update(item);
            }

            return item;
        }

        private IQueryable<EAMISDELIVERYRECEIPT> FilteredEntities(EamisDeliveryReceiptDTO filter, IQueryable<EAMISDELIVERYRECEIPT> custom_query = null, bool strict = false)
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
          

            var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisDeliveryReceiptDTO>> List(EamisDeliveryReceiptDTO filter, PageConfig config)
        {
            IQueryable<EAMISDELIVERYRECEIPT> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisDeliveryReceiptDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISDELIVERYRECEIPT> PagedQuery(IQueryable<EAMISDELIVERYRECEIPT> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x=> x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EamisDeliveryReceiptDTO> QueryToDTO (IQueryable<EAMISDELIVERYRECEIPT> query)
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
                }).Where(i => i.TransactionNumber == x.TRANSACTION_TYPE).ToList()
            });
        }

        public async Task<EamisDeliveryReceiptDTO> Update(EamisDeliveryReceiptDTO item)
        {
            EAMISDELIVERYRECEIPT data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<string> GetNextSequenceNumber()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumber(TransactionTypeSettings.DeliveryReceipt);
            return nextId;
        }
        private IQueryable<EAMISDELIVERYRECEIPT> PagedQueryForSearch(IQueryable<EAMISDELIVERYRECEIPT> query)
        {
            return query;
        }

        public async Task<DataList<EamisDeliveryReceiptDTO>> SearchDeliveryReceipt(string type, string searchValue)
        {
            IQueryable<EAMISDELIVERYRECEIPT> query = null;
            if (type == "Transaction #")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().Where(x => x.TRANSACTION_TYPE.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Received By")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().Where(x => x.RECEIVED_BY.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Date Received")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().Where(x => x.DATE_RECEIVED.ToString().Contains(searchValue)).AsQueryable();
            }
            else if (type == "Supplier DR")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().Where(x => x.SUPPLIER_GROUP.COMPANY_NAME.Contains(searchValue)).AsQueryable();
            }
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisDeliveryReceiptDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        public async Task<EamisDeliveryReceiptDTO> getDeliveryItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisDeliveryReceiptDTO
            {
                Id = result.ID,
                TransactionType = result.TRANSACTION_TYPE,
                ReceivedBy = result.RECEIVED_BY,
                DateReceived = result.DATE_RECEIVED,
                SupplierId = result.SUPPLIER_ID,
                PurchaseOrderNumber = result.PURCHASE_ORDER_NUMBER,
                PurchaseOrderDate = result.PURCHASE_ORDER_DATE,
                PurchaseRequestNumber = result.PURCHASE_REQUEST_NUMBER,
                PurchaseRequestDate = result.PURCHASE_REQUEST_DATE,
                SaleInvoiceNumber = result.SALE_INVOICE_NUMBER,
                SaleInvoiceDate = result.SALE_INVOICE_DATE,
                TotalAmount = result.TOTAL_AMOUNT,
                TransactionStatus = result.TRANSACTION_STATUS,
                StockroomId = result.WAREHOUSE_ID,
                DRNumFrSupplier = result.DR_BY_SUPPLIER_NUMBER,
                DRDate = result.DR_BY_SUPPLIER_DATE,
                AprDate = result.APR_DATE,
                AprNum = result.APR_NUMBER,
                Warehouse = _ctx.EAMIS_WAREHOUSE.AsNoTracking().Select(w => new EamisWarehouseDTO
                {
                    Id = w.ID,
                    Warehouse_Description = w.WAREHOUSE_DESCRIPTION,
                    Barangay_Code = w.BARANGAY_CODE,
                    IsActive = w.IS_ACTIVE,
                    Street_Name = w.STREET_NAME,

                }).FirstOrDefault(x => x.Id == result.WAREHOUSE_ID),
                Supplier = _ctx.EAMIS_SUPPLIER.AsNoTracking().Select(s => new EamisSupplierDTO
                {
                    Id = s.ID,
                    CompanyName = s.COMPANY_NAME
                }).FirstOrDefault(x => x.Id == result.SUPPLIER_ID),
                DeliveryReceiptDetails = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Select(d => new EamisDeliveryReceiptDetailsDTO
                {
                    Id = d.ID,
                    ItemId = d.ITEM_ID,
                    DeliveryReceiptId = d.DELIVERY_RECEIPT_ID,
                    QtyOrder = d.QTY_ORDER,
                    QtyDelivered = d.QTY_DELIVERED,
                    QtyRejected = d.QTY_REJECTED,
                    QtyReceived = d.QTY_RECEIVED,
                    UnitCost = d.UNIT_COST,
                    SubTotal = d.SUB_TOTAL,
                    SerialNumber = d.SERIAL_LOT,
                    UnitOfMeasurement = d.UNIT_OF_MEASUREMENT,
                    PropertyItem = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Select(p => new EamisPropertyItemsDTO
                    {
                        PropertyNo = p.PROPERTY_NO,
                        PropertyName = p.PROPERTY_NAME,
                        Id = p.ID
                    }).FirstOrDefault(x => x.Id == d.ITEM_ID),
                    PropertySerialTran = _ctx.EAMIS_SERIAL_TRAN.AsNoTracking().Select(s => new EamisSerialTranDTO
                    {
                        Id = s.ID,
                        SerialNumber = s.SERIAL_NO,
                        WarrantyExpiryDate = s.WARRANTY_EXPIRY_DATE,
                        DeliveryReceiptDetailsId = s.DELIVERY_RECEIPT_DETAILS_ID
                    }).Where(i => i.DeliveryReceiptDetailsId == d.ID).ToList()
                }).Where(drId => drId.DeliveryReceiptId == result.ID).ToList(),
                DeliveryImages = _ctx.EAMIS_ATTACHED_FILES.AsNoTracking().Select(v => new EamisAttachedFilesDTO
                {
                    Id = v.ID,
                    FileName = v.ATTACHED_FILENAME,
                    ModuleName = v.MODULE_NAME,
                    TransactionNumber = v.TRANID
                }).Where(i => i.TransactionNumber == result.TRANSACTION_TYPE).ToList()
            };
        }

        //added by me
        public async Task<List<LookupDTO>> ForRenewalTransactionNumber() 
        {
            DateTime currentDate = DateTime.Now;
            DateTime threeYearsAgo = currentDate.AddYears(-3);
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                   .Where(f => f.ACQUISITION_DATE <= threeYearsAgo)
                   .Join(_ctx.EAMIS_PROPERTY_TRANSACTION
                         .Where(t => t.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving), 
                         d => d.PROPERTY_TRANS_ID,
                         t => t.ID,
                         (d, t) => new { TransactionDetails = d, Transaction = t })
                   .GroupBy(x => x.TransactionDetails.PROPERTY_NUMBER)
                   .Select(i => new LookupDTO
                   {
                       TransactionNumber = i.Key
                   })
                   .ToList();

            return result;
        }


    }
}
