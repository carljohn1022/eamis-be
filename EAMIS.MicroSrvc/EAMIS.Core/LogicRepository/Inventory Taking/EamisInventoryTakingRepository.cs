using EAMIS.Common.DTO.Inventory_Taking;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.ContractRepository.Inventory_Taking;
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

namespace EAMIS.Core.LogicRepository.Inventory_Taking
{
    public class EamisInventoryTakingRepository : IEamisInventoryTakingRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;

        public EamisInventoryTakingRepository(EAMISContext ctx)
        {      
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        private EAMISINVENTORYTAKING MapToEntity(EamisInventoryTakingDTO item)
        {
            if (item == null) return new EAMISINVENTORYTAKING();
            return new EAMISINVENTORYTAKING
            {
                PROPERTY_NUMBER = item.PropertyNumber,
                SERIAL_NUMBER = item.SerialNumber,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                ACQUISITION_DATE = item.AcquisitionDate,
                REMARKS = item.Remarks,
                USER_STAMP = item.UserStamp
            };
        }
        public async Task<EamisInventoryTakingDTO> getScannedPropertyNumberList(string propertyNumber, string office, string department, string userStamp)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync(x => x.PROPERTY_NUMBER == propertyNumber)).ConfigureAwait(false);

            var trylang = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                    .OrderByDescending(x => x.ID)
                    .Where(x => x.ASSIGNEE_CUSTODIAN == 7 && !(_ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                        .Where(y => y.SERIAL_NUMBER == x.SERIAL_NUMBER && y.ASSIGNEE_CUSTODIAN != 7)
                        .Any()))
                    .Select(x => new {
                        x.ID,
                        x.PROPERTY_NUMBER,
                        x.ITEM_DESCRIPTION,
                        x.SERIAL_NUMBER
                    }).Distinct()
                    .ToList()).ConfigureAwait(false);

            if (result == null)
            {
                throw new Exception("Result is null.");
            }

            string remarks = "";
            if (result.OFFICE == office && result.DEPARTMENT == department)
            {
                remarks = "FOUND IN STATION";
            }
            else
            {
                remarks = "INCORRECT LOCATION/SHOULD BE IN " + result.OFFICE + " - " + result.DEPARTMENT;
            }

            var dto = new EamisInventoryTakingDTO
            {
                PropertyNumber = result.PROPERTY_NUMBER,
                SerialNumber = result.SERIAL_NUMBER,
                Office = office,
                Department = department,
                AcquisitionDate = result.ACQUISITION_DATE,
                Remarks = remarks,
                UserStamp = userStamp
            };

            // Map DTO to entity
            var entity = MapToEntity(dto);

            // Insert entity into table
            _ctx.Entry(entity).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            return dto;
        }
        private IQueryable<EAMISINVENTORYTAKING> FilteredEntities(EamisInventoryTakingDTO filter, IQueryable<EAMISINVENTORYTAKING> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISINVENTORYTAKING>(true);
           
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                   ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                   : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNumber)) predicate = (strict)
                    ? predicate.And(x => x.SERIAL_NUMBER.ToLower() == filter.SerialNumber.ToLower())
                    : predicate.And(x => x.SERIAL_NUMBER.Contains(filter.SerialNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                   ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                   : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                   ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                   : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
            if (!string.IsNullOrEmpty(filter.Remarks)) predicate = (strict)
                   ? predicate.And(x => x.REMARKS.ToLower() == filter.Remarks.ToLower())
                   : predicate.And(x => x.REMARKS.Contains(filter.Remarks.ToLower()));

            var query = custom_query ?? _ctx.EAMIS_INVENTORY_TAKING;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisInventoryTakingDTO>> List(EamisInventoryTakingDTO filter, PageConfig config)
        {
            IQueryable<EAMISINVENTORYTAKING> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisInventoryTakingDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISINVENTORYTAKING> PagedQuery(IQueryable<EAMISINVENTORYTAKING> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EamisInventoryTakingDTO> QueryToDTO(IQueryable<EAMISINVENTORYTAKING> query)
        {
            return query.Select(x => new EamisInventoryTakingDTO
            {
                PropertyNumber = x.PROPERTY_NUMBER,
                SerialNumber = x.SERIAL_NUMBER,
                AcquisitionDate = x.ACQUISITION_DATE,
                Office = x.OFFICE,
                Department = x.DEPARTMENT,
                Remarks = x.REMARKS
            });
        }
        public Task<bool> CheckPropertyNumberExist(string propertyNumber)
        {
            return _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().AnyAsync(x => x.PROPERTY_NUMBER == propertyNumber);
        }
        public Task<bool> CheckPropertyNumberExistIfScanned(string propertyNumber)
        {
            return _ctx.EAMIS_INVENTORY_TAKING.AsNoTracking().AnyAsync(x => x.PROPERTY_NUMBER == propertyNumber);
        }
        public async Task<List<EamisResponsibilityCenterDTO>> GetUnitDesc(bool isActive, string officeDesc)
        {
            var result = _ctx.EAMIS_RESPONSIBILITY_CENTER
                                          .Where(f => f.IS_ACTIVE == isActive && f.OFFICE_DESC == officeDesc )
                                          .GroupBy(x => x.UNIT_DESC)
                                          .Select(i => new EamisResponsibilityCenterDTO
                                          {
                                              unitDesc = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<EamisResponsibilityCenterDTO>> GetOfficeDesc(bool isActive)
        {
            var result = _ctx.EAMIS_RESPONSIBILITY_CENTER
                                          .Where(f => f.IS_ACTIVE == isActive)
                                          .GroupBy(x => x.OFFICE_DESC)
                                          .Select(i => new EamisResponsibilityCenterDTO
                                          {
                                              officeDesc = i.Key
                                          })
                                          .ToList();

            return result;
        }

        // Try lang
        //private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, string agencyEmployeeNumber, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> additional_query = null)
        //{
        //    var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
        //    //Do not display items under service logs


        //    var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
        //                                   .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
        //                                   d => d.PROPERTY_TRANS_ID,
        //                                   h => h.ID,
        //                                   (d, h) => new { d, h })
        //                                   .Where(x =>
        //                                          (x.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance))
        //                                   .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
        //                                   {
        //                                       ID = x.d.ID,
        //                                       PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
        //                                       IS_DEPRECIATION = x.d.IS_DEPRECIATION,
        //                                       DR = x.d.DR,
        //                                       PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
        //                                       ITEM_CODE = x.d.ITEM_CODE,
        //                                       ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
        //                                       SERIAL_NUMBER = x.d.SERIAL_NUMBER,
        //                                       PO = x.d.PO,
        //                                       PR = x.d.PR,
        //                                       ACQUISITION_DATE = x.d.ACQUISITION_DATE,
        //                                       ASSIGNEE_CUSTODIAN = x.d.ASSIGNEE_CUSTODIAN,
        //                                       REQUESTED_BY = x.d.REQUESTED_BY,
        //                                       OFFICE = x.d.OFFICE,
        //                                       DEPARTMENT = x.d.DEPARTMENT,
        //                                       RESPONSIBILITY_CODE = x.d.RESPONSIBILITY_CODE,
        //                                       UNIT_COST = x.d.UNIT_COST,
        //                                       QTY = x.d.QTY,
        //                                       SALVAGE_VALUE = x.d.SALVAGE_VALUE,
        //                                       BOOK_VALUE = x.d.BOOK_VALUE,
        //                                       ESTIMATED_LIFE = x.d.ESTIMATED_LIFE,
        //                                       AREA = x.d.AREA,
        //                                       SEMI_EXPANDABLE_AMOUNT = x.d.SEMI_EXPANDABLE_AMOUNT,
        //                                       USER_STAMP = x.d.USER_STAMP,
        //                                       TIME_STAMP = x.d.TIME_STAMP,
        //                                       WARRANTY_EXPIRY = x.d.WARRANTY_EXPIRY,
        //                                       INVOICE = x.d.INVOICE,
        //                                       PROPERTY_CONDITION = x.d.PROPERTY_CONDITION,
        //                                   });

        //    return query.Where(predicate);
            
        //}


        //public async Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string agencyEmployeeNumber)
        //{
        //    IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredItemsForReceiving(filter, agencyEmployeeNumber);
        //    string resolved_sort = config.SortBy ?? "Id";
        //    bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
        //    int resolved_size = config.Size ?? _maxPageSize;
        //    if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
        //    int resolved_index = config.Index ?? 1;


        //    var paged = PagedQueryItemsForReceiving(query, resolved_size, resolved_index);
        //    var result = new DataList<EamisPropertyTransactionDetailsDTO>
        //    {
        //        Count = await query.CountAsync(),
        //        Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
        //    };
        //    return result;
        //}
        //private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        //{
        //    return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        //}
        //private IQueryable<EamisPropertyTransactionDetailsDTO> QueryToDTOItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        //{
        //    return query.Select(x => new EamisPropertyTransactionDetailsDTO
        //    {
        //        Id = x.ID,
        //        PropertyTransactionID = x.PROPERTY_TRANS_ID,
        //        isDepreciation = x.IS_DEPRECIATION,
        //        ItemCode = x.ITEM_CODE,
        //        Dr = x.DR,
        //        PropertyNumber = x.PROPERTY_NUMBER,
        //        ItemDescription = x.ITEM_DESCRIPTION,
        //        SerialNumber = x.SERIAL_NUMBER,
        //        Po = x.PO,
        //        Pr = x.PR,
        //        AcquisitionDate = x.ACQUISITION_DATE,
        //        AssigneeCustodian = x.ASSIGNEE_CUSTODIAN,
        //        RequestedBy = x.REQUESTED_BY,
        //        Office = x.OFFICE,
        //        Department = x.DEPARTMENT,
        //        ResponsibilityCode = x.RESPONSIBILITY_CODE,
        //        UnitCost = x.UNIT_COST,
        //        Qty = x.QTY,
        //        IssuedQty = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
        //                        .Where(r => r.REFERENCE_ID == x.ID)
        //                        .GroupBy(g => g.REFERENCE_ID)
        //                        .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
        //        RemainingQty = x.QTY -
        //                        _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
        //                        .Where(r => r.REFERENCE_ID == x.ID)
        //                        .GroupBy(g => g.REFERENCE_ID)
        //                        .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
        //        //IsAssigneeCustodianChange = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
        //        //                .Where(r => r.REFERENCE_ID == x.ID )
        //        //                .GroupBy(g => g.REFERENCE_ID)
        //        //                .Select(i => i.Any(v => v.ASSIGNEE_CUSTODIAN != x.ASSIGNEE_CUSTODIAN))
        //        //                .FirstOrDefault() ? "yes" : "no",
        //        SalvageValue = x.SALVAGE_VALUE,
        //        BookValue = x.BOOK_VALUE,
        //        EstLife = x.ESTIMATED_LIFE,
        //        Area = x.AREA,
        //        Semi = x.SEMI_EXPANDABLE_AMOUNT,
        //        WarrantyExpiry = x.WARRANTY_EXPIRY,
        //        UserStamp = x.USER_STAMP,
        //        TimeStamp = x.TIME_STAMP,
        //        Invoice = x.INVOICE,
        //        PropertyCondition = x.PROPERTY_CONDITION,
        //        transactionDetailId = x.REFERENCE_ID,
        //        PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(x => new EamisPropertyTransactionDTO
        //        {
        //            Id = x.ID,
        //            TransactionStatus = x.TRANSACTION_STATUS,
        //            TransactionType = x.TRANSACTION_TYPE,
        //            TransactionNumber = x.TRANSACTION_NUMBER,
        //            TranType = x.TRAN_TYPE,
        //            TransactionDate = x.TRANSACTION_DATE,
        //            FundSource = x.FUND_SOURCE,
        //            FiscalPeriod = x.FISCALPERIOD,
        //            ReceivedBy = x.RECEIVED_BY,
        //            BranchID = x.BRANCH_ID
        //        }).Where(i => i.Id == x.PROPERTY_TRANS_ID).FirstOrDefault()

        //    }

        //    );
        //}




    }
}
