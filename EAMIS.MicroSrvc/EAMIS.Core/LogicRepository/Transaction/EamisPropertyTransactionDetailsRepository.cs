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
                PROPERTY_TRANS_ID = item.PropertyTransactionID,
                IS_DEPRECIATION = item.isDepreciation,
                DR = item.Dr,
                PROPERTY_NUMBER = item.PropertyNumber,
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
                PropertyTransactionID = x.PROPERTY_TRANS_ID,
                isDepreciation = x.IS_DEPRECIATION,
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
                UserStamp = x.USER_STAMP,
                TimeStamp = x.TIME_STAMP,
                WarrantyExpiry = x.WARRANTY_EXPIRY,
                Invoice = x.INVOICE,
                PropertyCondition = x.PROPERTY_CONDITION,
                
                PropertyTransactionGroup  = new EamisPropertyTransactionDTO
                {
                    Id = x.PROPERTY_TRANSACTION_GROUP.ID,
                    TransactionStatus = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_STATUS,
                    TransactionNumber = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER,
                    TransactionDate = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_DATE,
                    FiscalPeriod = x.PROPERTY_TRANSACTION_GROUP.FISCALPERIOD,
                    ReceivedBy = x.PROPERTY_TRANSACTION_GROUP.RECEIVED_BY,
                }

            });
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredEntities(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.isDepreciation != null && filter.isDepreciation != false)
                    predicate = predicate.And(x => x.IS_DEPRECIATION == filter.isDepreciation);
            if (!string.IsNullOrEmpty(filter.Dr)) predicate = (strict)
                     ? predicate.And(x => x.DR.ToLower() == filter.Dr.ToLower())
                     : predicate.And(x => x.DR.Contains(filter.Dr.ToLower()));
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                     : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.ItemDescription)) predicate = (strict)
                     ? predicate.And(x => x.ITEM_DESCRIPTION.ToLower() == filter.ItemDescription.ToLower())
                     : predicate.And(x => x.ITEM_DESCRIPTION.Contains(filter.ItemDescription.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNumber)) predicate = (strict)
                     ? predicate.And(x => x.SERIAL_NUMBER.ToLower() == filter.SerialNumber.ToLower())
                     : predicate.And(x => x.SERIAL_NUMBER.Contains(filter.SerialNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.Po)) predicate = (strict)
                     ? predicate.And(x => x.PO.ToLower() == filter.Po.ToLower())
                     : predicate.And(x => x.PO.Contains(filter.Po.ToLower()));
            if (!string.IsNullOrEmpty(filter.Pr)) predicate = (strict)
                     ? predicate.And(x => x.PR.ToLower() == filter.Pr.ToLower())
                     : predicate.And(x => x.PR.Contains(filter.Pr.ToLower()));
            if (filter.AcquisitionDate != null && filter.AcquisitionDate != DateTime.MinValue)
                predicate = predicate.And(x => x.ACQUISITION_DATE == filter.AcquisitionDate);
            if (filter.AssigneeCustodian != null && filter.AssigneeCustodian != 0)
                predicate = predicate.And(x => x.ASSIGNEE_CUSTODIAN == filter.AssigneeCustodian);
            if (!string.IsNullOrEmpty(filter.RequestedBy)) predicate = (strict)
                     ? predicate.And(x => x.REQUESTED_BY.ToLower() == filter.RequestedBy.ToLower())
                     : predicate.And(x => x.REQUESTED_BY.Contains(filter.RequestedBy.ToLower()));
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                     ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                     : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                     ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                     : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
            if (!string.IsNullOrEmpty(filter.ResponsibilityCode)) predicate = (strict)
                     ? predicate.And(x => x.RESPONSIBILITY_CODE.ToLower() == filter.ResponsibilityCode.ToLower())
                     : predicate.And(x => x.RESPONSIBILITY_CODE.Contains(filter.ResponsibilityCode.ToLower()));
            if (filter.UnitCost != null && filter.UnitCost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.UnitCost);
            if (filter.Qty != null && filter.Qty != 0)
                predicate = predicate.And(x => x.QTY == filter.Qty);
            if (filter.SalvageValue != null && filter.SalvageValue != 0)
                predicate = predicate.And(x => x.SALVAGE_VALUE == filter.SalvageValue);
            if (filter.BookValue != null && filter.BookValue != 0)
                predicate = predicate.And(x => x.BOOK_VALUE == filter.BookValue);
            if (filter.EstLife != null && filter.EstLife != 0)
                predicate = predicate.And(x => x.ESTIMATED_LIFE == filter.EstLife);
            if (filter.Area != null && filter.Area != 0)
                predicate = predicate.And(x => x.AREA == filter.Area);
            if (filter.Semi != null && filter.Semi != 0)
                predicate = predicate.And(x => x.SEMI_EXPANDABLE_AMOUNT == filter.Semi);
            if (!string.IsNullOrEmpty(filter.UserStamp)) predicate = (strict)
                    ? predicate.And(x => x.USER_STAMP.ToLower() == filter.UserStamp.ToLower())
                    : predicate.And(x => x.USER_STAMP.Contains(filter.UserStamp.ToLower()));
            if (filter.TimeStamp != null && filter.TimeStamp != DateTime.MinValue)
                predicate = predicate.And(x => x.TIME_STAMP == filter.TimeStamp);
            if (filter.WarrantyExpiry != null && filter.WarrantyExpiry != DateTime.MinValue)
                predicate = predicate.And(x => x.WARRANTY_EXPIRY == filter.WarrantyExpiry);
            if (!string.IsNullOrEmpty(filter.Invoice)) predicate = (strict)
                    ? predicate.And(x => x.INVOICE.ToLower() == filter.Invoice.ToLower())
                    : predicate.And(x => x.INVOICE.Contains(filter.Invoice.ToLower()));
            if (!string.IsNullOrEmpty(filter.PropertyCondition)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_CONDITION.ToLower() == filter.PropertyCondition.ToLower())
                     : predicate.And(x => x.PROPERTY_CONDITION.Contains(filter.PropertyCondition.ToLower()));

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
        public async Task<EamisPropertyTransactionDetailsDTO> getPropertyItemById(int itemID)
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().FirstOrDefault(x => x.ID == itemID);
            return new EamisPropertyTransactionDetailsDTO {
                Id = result.ID,
                PropertyTransactionID = result.PROPERTY_TRANS_ID,
                isDepreciation = result.IS_DEPRECIATION,
                Dr = result.DR,
                PropertyNumber = result.PROPERTY_NUMBER,
                ItemDescription = result.ITEM_DESCRIPTION,
                SerialNumber = result.SERIAL_NUMBER,
                Po = result.PO,
                Pr = result.PR,
                AcquisitionDate = result.ACQUISITION_DATE,
                AssigneeCustodian = result.ASSIGNEE_CUSTODIAN,
                RequestedBy = result.REQUESTED_BY,
                Office = result.OFFICE,
                Department = result.DEPARTMENT,
                ResponsibilityCode = result.RESPONSIBILITY_CODE,
                UnitCost = result.UNIT_COST,
                Qty = result.QTY,
                SalvageValue = result.SALVAGE_VALUE,
                BookValue = result.BOOK_VALUE,
                EstLife = result.ESTIMATED_LIFE,
                Area = result.AREA,
                Semi = result.SEMI_EXPANDABLE_AMOUNT,
                UserStamp = result.USER_STAMP,
                TimeStamp = result.TIME_STAMP,
                WarrantyExpiry = result.WARRANTY_EXPIRY,
                Invoice = result.INVOICE,
                PropertyCondition = result.PROPERTY_CONDITION
            };
        }
    }
}
