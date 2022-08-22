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

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisDeliveryReceiptDetailsRepository : IEamisDeliveryReceiptDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEamisPropertyTransactionDetailsRepository _eamisPropertyTransactionDetailsRepository;
        private readonly IEamisPropertyRevalutionRepository _eamisPropertyRevalutionRepository;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisDeliveryReceiptDetailsRepository(EAMISContext ctx,
            IEamisPropertyTransactionDetailsRepository eamisPropertyTransactionDetailsRepository,
            IEamisPropertyRevalutionRepository eamisPropertyRevalutionRepository)
        {
            _ctx = ctx;
            _eamisPropertyTransactionDetailsRepository = eamisPropertyTransactionDetailsRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
            _eamisPropertyRevalutionRepository = eamisPropertyRevalutionRepository;
        }

        public async Task<EamisDeliveryReceiptDetailsDTO> Delete(EamisDeliveryReceiptDetailsDTO item)
        {
            EAMISDELIVERYRECEIPTDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISDELIVERYRECEIPTDETAILS MapToEntity(EamisDeliveryReceiptDetailsDTO item)
        {
            if (item == null) return new EAMISDELIVERYRECEIPTDETAILS();
            return new EAMISDELIVERYRECEIPTDETAILS
            {
                ID = item.Id,
                DELIVERY_RECEIPT_ID = item.DeliveryReceiptId,
                ITEM_ID = item.ItemId,
                ITEM_CODE = item.ItemCode,
                ITEM_DESCRIPTION = item.ItemDescription,
                QTY_ORDER = item.QtyOrder,
                QTY_DELIVERED = item.QtyDelivered,
                QTY_RECEIVED = item.QtyReceived,
                QTY_REJECTED = item.QtyRejected,
                UNIT_COST = item.UnitCost,
                SERIAL_LOT = item.SerialNumber,
                UNIT_OF_MEASUREMENT = item.UnitOfMeasurement,
                SUB_TOTAL = item.SubTotal
                //WARRANTY_EXPIRY_DATE = item.WarrantyExpiryDate
            };
        }

        

        public async Task<EamisDeliveryReceiptDetailsDTO> Insert(EamisDeliveryReceiptDetailsDTO item)
        {
            EAMISDELIVERYRECEIPTDETAILS data = MapToEntity(item);
            var transaction = _ctx.Database.BeginTransaction();
            try
            {
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();

                //update property_items table quantity in stock
                await _eamisPropertyTransactionDetailsRepository.UpdatePropertyItemQty(item);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _errorMessage = ex.Message;
                bolerror = true;
            }
            return item;
        }

        public async Task<DataList<EamisDeliveryReceiptDetailsDTO>> List(EamisDeliveryReceiptDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISDELIVERYRECEIPTDETAILS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisDeliveryReceiptDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisDeliveryReceiptDetailsDTO> QueryToDTO(IQueryable<EAMISDELIVERYRECEIPTDETAILS> query)
        {
            return query.Select(x => new EamisDeliveryReceiptDetailsDTO
            {
                Id = x.ID,
                DeliveryReceiptId = x.DELIVERY_RECEIPT_ID,
                ItemId = x.ITEM_ID,
                //ItemCode = x.ITEM_CODE,
                // ItemDescription = x.ITEM_DESCRIPTION,
                QtyOrder = x.QTY_ORDER,
                QtyDelivered = x.QTY_DELIVERED,
                QtyRejected = x.QTY_REJECTED,
                QtyReceived = x.QTY_RECEIVED,
                UnitCost = x.UNIT_COST,
                SerialNumber = x.SERIAL_LOT,
                UnitOfMeasurement = x.UNIT_OF_MEASUREMENT,
                SubTotal = x.SUB_TOTAL,
                //WarrantyExpiryDate = x.WARRANTY_EXPIRY_DATE,
                PropertyItem = new EamisPropertyItemsDTO
                {
                    Id = x.ITEMS_GROUP.ID,
                    PropertyNo = x.ITEMS_GROUP.PROPERTY_NO,
                    PropertyName = x.ITEMS_GROUP.PROPERTY_NAME,
                    UomId = x.ITEMS_GROUP.UOM_ID,
                },
                DeliveryReceipt = new EamisDeliveryReceiptDTO
                {
                    Id = x.DELIVERY_RECEIPT_GROUP.ID,
                    PurchaseOrderNumber = x.DELIVERY_RECEIPT_GROUP.PURCHASE_ORDER_NUMBER,
                    PurchaseRequestNumber = x.DELIVERY_RECEIPT_GROUP.PURCHASE_REQUEST_NUMBER,
                    TransactionType = x.DELIVERY_RECEIPT_GROUP.TRANSACTION_TYPE,
                    ReceivedBy = x.DELIVERY_RECEIPT_GROUP.RECEIVED_BY,
                    DateReceived = x.DELIVERY_RECEIPT_GROUP.DATE_RECEIVED,
                    SupplierId = x.DELIVERY_RECEIPT_GROUP.SUPPLIER_ID,
                    StockroomId = x.DELIVERY_RECEIPT_GROUP.WAREHOUSE_ID,
                    SaleInvoiceNumber = x.DELIVERY_RECEIPT_GROUP.SALE_INVOICE_NUMBER,

                }
            });
        }

        private IQueryable<EAMISDELIVERYRECEIPTDETAILS> PagedQuery(IQueryable<EAMISDELIVERYRECEIPTDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISDELIVERYRECEIPTDETAILS> FilteredEntities(EamisDeliveryReceiptDetailsDTO filter, IQueryable<EAMISDELIVERYRECEIPTDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISDELIVERYRECEIPTDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.DeliveryReceiptId != null && filter.DeliveryReceiptId != 0)
                predicate = predicate.And(x => x.DELIVERY_RECEIPT_ID == filter.DeliveryReceiptId);
            if (filter.ItemId != null && filter.ItemId != 0)
                predicate = predicate.And(x => x.ITEM_ID == filter.ItemId);
            if (filter.QtyOrder != null && filter.QtyOrder != 0)
                predicate = predicate.And(x => x.QTY_ORDER == filter.QtyOrder);
            if (filter.QtyDelivered != null && filter.QtyDelivered != 0)
                predicate = predicate.And(x => x.QTY_DELIVERED == filter.QtyDelivered);
            if (filter.QtyRejected != null && filter.QtyRejected != 0)
                predicate = predicate.And(x => x.QTY_REJECTED == filter.QtyRejected);
            if (filter.QtyReceived != null && filter.QtyReceived != 0)
                predicate = predicate.And(x => x.QTY_RECEIVED == filter.QtyReceived);
            if (filter.UnitCost != null && filter.UnitCost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.UnitCost);
            if (filter.SerialNumber != null && filter.SerialNumber != 0)
                predicate = predicate.And(x => x.SERIAL_LOT == filter.SerialNumber);

            if (!string.IsNullOrEmpty(filter.UnitOfMeasurement)) predicate = (strict)
                  ? predicate.And(x => x.UNIT_OF_MEASUREMENT.ToLower() == filter.UnitOfMeasurement.ToLower())
                  : predicate.And(x => x.UNIT_OF_MEASUREMENT.Contains(filter.UnitOfMeasurement.ToLower()));
            //if (filter.WarrantyExpiryDate != null && filter.WarrantyExpiryDate != DateTime.MinValue)
            //    predicate = predicate.And(x => x.WARRANTY_EXPIRY_DATE == filter.WarrantyExpiryDate);
            var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS;
            return query.Where(predicate);
        }

        public async Task<EamisDeliveryReceiptDetailsDTO> Update(EamisDeliveryReceiptDetailsDTO item)
        {
            EAMISDELIVERYRECEIPTDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = data.ID == 0 ? EntityState.Added : EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<string> GetItemById(int itemId)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.Where(s => s.ID == itemId).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].PROPERTY_NO.ToString() + ":" +
                           result[0].PROPERTY_NAME.ToString();
            }
            return retValue;
        }
    }
}
