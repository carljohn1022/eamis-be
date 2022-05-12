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

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisDeliveryReceiptRepository : IEamisDeliveryReceiptRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisDeliveryReceiptRepository(EAMISContext ctx)
        {
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
                SUPPLIER = item.Supplier,
                PURCHASE_ORDER_NUMBER = item.PurchaseOrderNumber,
                PURCHASE_ORDER_DATE = item.PurchaseOrderDate,
                PURCHASE_REQUEST_NUMBER = item.PurchaseRequestNumber,
                PURCHASE_REQUEST_DATE = item.PurchaseRequestDate,
                SALE_INVOICE_NUMBER = item.SaleInvoiceNumber,
                SALE_INVOICE_DATE = item.SaleInvoiceDate,
                TOTAL_AMOUNT = item.TotalAmount,
                TRANSACTION_STATUS = item.TransactionStatus,
                SERIAL_LOT = item.SerialLot
            };
        }

        public async Task<EamisDeliveryReceiptDTO> Insert(EamisDeliveryReceiptDTO item)
        {
            EAMISDELIVERYRECEIPT data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisDeliveryReceiptDTO>> List(EamisDeliveryReceiptDTO filter, PageConfig config)
        {
            IQueryable<EAMISDELIVERYRECEIPT> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisDeliveryReceiptDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisDeliveryReceiptDTO> QueryToDTO(IQueryable<EAMISDELIVERYRECEIPT> query)
        {
            return query.Select(x => new EamisDeliveryReceiptDTO
            {
                Id = x.ID,
                TransactionType = x.TRANSACTION_TYPE,
                Supplier = x.SUPPLIER,
                PurchaseOrderNumber = x.PURCHASE_ORDER_NUMBER,
                PurchaseOrderDate = x.PURCHASE_ORDER_DATE,
                PurchaseRequestNumber = x.PURCHASE_REQUEST_NUMBER,
                PurchaseRequestDate = x.PURCHASE_REQUEST_DATE,
                SaleInvoiceNumber = x.SALE_INVOICE_NUMBER,
                SaleInvoiceDate = x.SALE_INVOICE_DATE,
                TotalAmount = x.TOTAL_AMOUNT,
                TransactionStatus = x.TRANSACTION_STATUS,
                SerialLot = x.SERIAL_LOT
            });
        }

        private IQueryable<EAMISDELIVERYRECEIPT> PagedQuery(IQueryable<EAMISDELIVERYRECEIPT> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISDELIVERYRECEIPT> FilteredEntities(EamisDeliveryReceiptDTO filter, IQueryable<EAMISDELIVERYRECEIPT> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISDELIVERYRECEIPT>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.TransactionType)) predicate = (strict)
                   ? predicate.And(x => x.TRANSACTION_TYPE.ToLower() == filter.TransactionType.ToLower())
                   : predicate.And(x => x.TRANSACTION_TYPE.Contains(filter.TransactionType.ToLower()));
            if (!string.IsNullOrEmpty(filter.Supplier)) predicate = (strict)
                   ? predicate.And(x => x.SUPPLIER.ToLower() == filter.Supplier.ToLower())
                   : predicate.And(x => x.SUPPLIER.Contains(filter.Supplier.ToLower()));
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
            if (filter.SerialLot != null && filter.SerialLot != 0)
                predicate = predicate.And(x => x.SERIAL_LOT == filter.SerialLot);
            var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT;
            return query.Where(predicate);
        }

        public async Task<EamisDeliveryReceiptDTO> Update(EamisDeliveryReceiptDTO item)
        {
            EAMISDELIVERYRECEIPT data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
