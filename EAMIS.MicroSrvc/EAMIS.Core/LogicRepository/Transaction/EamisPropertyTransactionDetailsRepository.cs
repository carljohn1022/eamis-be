using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
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

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyTransactionDetailsRepository : IEamisPropertyTransactionDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisPropertyTransactionDetailsRepository(EAMISContext ctx )
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item; ;
        }

        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTIONDETAILS();
            return new EAMISPROPERTYTRANSACTIONDETAILS
            {
                ID = item.Id,
                PROPERTY_TRANSACTION_ID = item.PropertyTransactionId,
                PROPERTY_NUMBER = item.PropertyNumber,
                PROPERTY_DESCRIPTION = item.PropertyDescription,
                RECEIVING_DATE = item.ReceivingDate,
                DELIVERY_RECEIPT_NUMBER = item.DeliveryReceiptNumber,
                PURCHASE_ORDER_NUMBER = item.PurchaseOrderNumber,
                PURCHASE_ORDER_DATE = item.PurchaseOrderDate,
                PURCHASE_REQUEST_NUMBER = item.PurchaseRequestNumber,
                PURCHASE_REQUEST_DATE = item.PurchaseRequestDate,
                SERIAL_NUMBER = item.SerialNumber,
                CUSTODIAN_ID = item.CustodianId,
                DEPARTMENT = item.Department,
                QTY = item.Qty,
                UNIT_COST = item.UnitCost,
                SALVAGE_VALUE = item.SalvageValue,
                ESTIMATED_LIFE = item.EstimatedLife,
                BOOK_VALUE = item.BookValue,
                FORD_DEPRECIATION = item.FordDepreciation,
                USER_STAMP = item.UserStamp,
                TIME_STAMP = item.TimeStamp,
                ITEM_ID = item.ItemId,
                WARRANTY_START_DATE = item.WarrantyStartDate,
                PROPERTY_CONDITION = item.PropertyCondition,
                SEMI_EXPANDABLE_AMOUNT = item.SemiExpandableAmount,
                PROPERTY_KIT_ID = item.PropertyKitId,
                AREA_SQM = item.AreaSqm,
                OFFICE = item.Office
            };
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Insert(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> List(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisPropertyTransactionDetailsDTO> QueryToDTO(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransactionDetailsDTO
            {
                Id = x.ID,
                PropertyTransactionId = x.PROPERTY_TRANSACTION_ID,
                PropertyNumber = x.PROPERTY_NUMBER,
                PropertyDescription = x.PROPERTY_DESCRIPTION,
                ReceivingDate = x.RECEIVING_DATE,
                DeliveryReceiptNumber = x.DELIVERY_RECEIPT_NUMBER,
                PurchaseOrderNumber = x.PURCHASE_ORDER_NUMBER,
                PurchaseOrderDate = x.PURCHASE_ORDER_DATE,
                PurchaseRequestNumber = x.PURCHASE_REQUEST_NUMBER,
                PurchaseRequestDate = x.PURCHASE_REQUEST_DATE,
                SerialNumber = x.SERIAL_NUMBER,
                CustodianId = x.CUSTODIAN_ID,
                Department = x.DEPARTMENT,
                Qty = x.QTY,
                UnitCost = x.UNIT_COST,
                SalvageValue = x.SALVAGE_VALUE,
                EstimatedLife = x.ESTIMATED_LIFE,
                BookValue = x.BOOK_VALUE,
                FordDepreciation = x.FORD_DEPRECIATION,
                UserStamp = x.USER_STAMP,
                TimeStamp = x.TIME_STAMP,
                ItemId = x.ITEM_ID,
                WarrantyStartDate = x.WARRANTY_START_DATE,
                PropertyCondition = x.PROPERTY_CONDITION,
                SemiExpandableAmount = x.SEMI_EXPANDABLE_AMOUNT,
                PropertyKitId = x.PROPERTY_KIT_ID,
                AreaSqm = x.AREA_SQM,
                Office = x.OFFICE

            });
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredEntities(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.PropertyTransactionId != null && filter.PropertyTransactionId != 0)
                predicate = predicate.And(x => x.PROPERTY_TRANSACTION_ID == filter.PropertyTransactionId);
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                     : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.PropertyDescription)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_DESCRIPTION.ToLower() == filter.PropertyDescription.ToLower())
                     : predicate.And(x => x.PROPERTY_DESCRIPTION.Contains(filter.PropertyDescription.ToLower()));
            if (filter.ReceivingDate != null && filter.ReceivingDate != DateTime.MinValue)
                predicate = predicate.And(x => x.RECEIVING_DATE == filter.ReceivingDate);
            if (!string.IsNullOrEmpty(filter.DeliveryReceiptNumber)) predicate = (strict)
                     ? predicate.And(x => x.DELIVERY_RECEIPT_NUMBER.ToLower() == filter.DeliveryReceiptNumber.ToLower())
                     : predicate.And(x => x.DELIVERY_RECEIPT_NUMBER.Contains(filter.DeliveryReceiptNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.PurchaseRequestNumber)) predicate = (strict)
                     ? predicate.And(x => x.PURCHASE_REQUEST_NUMBER.ToLower() == filter.PurchaseRequestNumber.ToLower())
                     : predicate.And(x => x.PURCHASE_REQUEST_NUMBER.Contains(filter.PurchaseRequestNumber.ToLower()));
            if (filter.PurchaseRequestDate != null && filter.PurchaseRequestDate != DateTime.MinValue)
                predicate = predicate.And(x => x.PURCHASE_REQUEST_DATE == filter.PurchaseRequestDate);
            if (!string.IsNullOrEmpty(filter.PurchaseOrderNumber)) predicate = (strict)
                     ? predicate.And(x => x.PURCHASE_ORDER_NUMBER.ToLower() == filter.PurchaseOrderNumber.ToLower())
                     : predicate.And(x => x.PURCHASE_ORDER_NUMBER.Contains(filter.PurchaseOrderNumber.ToLower()));
            if (filter.PurchaseOrderDate != null && filter.PurchaseOrderDate != DateTime.MinValue)
                predicate = predicate.And(x => x.PURCHASE_ORDER_DATE == filter.PurchaseOrderDate);
            if (!string.IsNullOrEmpty(filter.SerialNumber)) predicate = (strict)
                     ? predicate.And(x => x.SERIAL_NUMBER.ToLower() == filter.SerialNumber.ToLower())
                     : predicate.And(x => x.SERIAL_NUMBER.Contains(filter.SerialNumber.ToLower()));
            if (filter.CustodianId != null && filter.CustodianId != 0)
                predicate = predicate.And(x => x.CUSTODIAN_ID == filter.CustodianId);
            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                     ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                     : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
            if (filter.Qty != null && filter.Qty != 0)
                predicate = predicate.And(x => x.QTY == filter.Qty);
            if (filter.UnitCost != null && filter.UnitCost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.UnitCost);
            if (filter.SalvageValue != null && filter.SalvageValue != 0)
                predicate = predicate.And(x => x.SALVAGE_VALUE == filter.SalvageValue);
            if (filter.EstimatedLife != null && filter.EstimatedLife != 0)
                predicate = predicate.And(x => x.ESTIMATED_LIFE == filter.EstimatedLife);
            if (filter.BookValue != null && filter.BookValue != 0)
                predicate = predicate.And(x => x.BOOK_VALUE == filter.BookValue);
            if (filter.FordDepreciation != null && filter.FordDepreciation != 0)
                predicate = predicate.And(x => x.FORD_DEPRECIATION == filter.FordDepreciation);
            if (!string.IsNullOrEmpty(filter.UserStamp)) predicate = (strict)
                    ? predicate.And(x => x.USER_STAMP.ToLower() == filter.UserStamp.ToLower())
                    : predicate.And(x => x.USER_STAMP.Contains(filter.UserStamp.ToLower()));
            if (filter.TimeStamp != null && filter.TimeStamp != DateTime.MinValue)
                predicate = predicate.And(x => x.TIME_STAMP == filter.TimeStamp);
            if (filter.ItemId != null && filter.ItemId != 0)
                predicate = predicate.And(x => x.ITEM_ID == filter.ItemId);
            if (filter.WarrantyStartDate != null && filter.WarrantyStartDate != DateTime.MinValue)
                predicate = predicate.And(x => x.WARRANTY_START_DATE == filter.WarrantyStartDate);
            if (!string.IsNullOrEmpty(filter.PropertyCondition)) predicate = (strict)
                    ? predicate.And(x => x.PROPERTY_CONDITION.ToLower() == filter.PropertyCondition.ToLower())
                    : predicate.And(x => x.PROPERTY_CONDITION.Contains(filter.PropertyCondition.ToLower()));
            if (filter.SemiExpandableAmount != null && filter.SemiExpandableAmount != 0)
                predicate = predicate.And(x => x.SEMI_EXPANDABLE_AMOUNT == filter.SemiExpandableAmount);
            if (filter.PropertyKitId != null && filter.PropertyKitId != 0)
                predicate = predicate.And(x => x.PROPERTY_KIT_ID == filter.PropertyKitId);
            if (filter.AreaSqm != null && filter.AreaSqm != 0)
                predicate = predicate.And(x => x.AREA_SQM == filter.AreaSqm);
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                    ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                    : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Update(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
