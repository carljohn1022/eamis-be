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
                WAREHOUSE_ID = item.StockroomId
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

            string _drType = item.TransactionType.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0');

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
    }
}
