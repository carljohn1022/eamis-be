﻿using EAMIS.Common.DTO.Transaction;
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
    public class EamisPropertyDisposalRepository : IEamisPropertyDisposalRepository
    {
        private readonly EAMISContext _ctx;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        private readonly int _maxPageSize;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }

        public EamisPropertyDisposalRepository(EAMISContext ctx, IEAMISIDProvider EAMISIDProvider)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        #region property transaction
        public async Task<string> GetNextSequenceNumber(string branchID)
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.PropertyDisposal, branchID);
            return nextId;
        }
        public async Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config, string branchID)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = FilteredEntites(filter, branchID);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyTransactionDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISPROPERTYTRANSACTION> FilteredEntites(EamisPropertyTransactionDTO filter, string branchID, IQueryable<EAMISPROPERTYTRANSACTION> custom_query = null, bool strict = false)
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
            if (!string.IsNullOrEmpty(filter.TransactionStatus)) predicate = (strict)
                   ? predicate.And(x => x.TRANSACTION_STATUS.ToLower() == filter.TransactionStatus.ToLower())
                   : predicate.And(x => x.TRANSACTION_STATUS.Contains(filter.TransactionStatus.ToLower()));
            if (!string.IsNullOrEmpty(filter.BranchID)) predicate = (strict)
                  ? predicate.And(x => x.BRANCH_ID.ToLower() == filter.BranchID.ToLower())
                  : predicate.And(x => x.BRANCH_ID.Contains(filter.BranchID.ToLower()));
            if (!string.IsNullOrEmpty(filter.UserStamp)) predicate = (strict)
                ? predicate.And(x => x.USER_STAMP.ToLower() == filter.UserStamp.ToLower())
                : predicate.And(x => x.USER_STAMP.Contains(filter.UserStamp.ToLower()));
            predicate = predicate.And(x => x.TRANSACTION_TYPE == TransactionTypeSettings.PropertyDisposal);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION;
            return query.Where(predicate);
        }
        private IQueryable<EAMISPROPERTYTRANSACTION> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTION> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
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
                TransactionStatus = x.TRANSACTION_STATUS
            });
        }
        public async Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item)
        {
            try
            {
                var result = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(i => i.TRANSACTION_NUMBER == item.TransactionNumber).FirstOrDefault();
                if (result != null)
                {
                    var nextIdProvided = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.PropertyDisposal, item.BranchID);
                    item.TransactionNumber = nextIdProvided;
                }

                EAMISPROPERTYTRANSACTION data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();

                //ensure that recently added record has the correct transaction type number
                item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

                //string _drType = PrefixSettings.PDPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

                //if (item.TransactionNumber != _drType)
                //{
                //    item.TransactionNumber = _drType; //if not matched, replace value of FTT with  ATT

                //    //reset context state to avoid error
                //    _ctx.Entry(data).State = EntityState.Detached;

                //    //call the update method, force to update the transaction type in the DB
                //    await this.Update(item);
                //}
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
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
                FiscalPeriod = result.FISCALPERIOD,
                TransactionType = result.TRANSACTION_TYPE,
                Memo = result.MEMO,
                ReceivedBy = result.RECEIVED_BY,
                ApprovedBy = result.APPROVED_BY,
                DeliveryDate = result.DELIVERY_DATE,
                UserStamp = result.USER_STAMP,
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
                    ModeOfDisposal = x.MODE_OF_DISPOSAL,
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

                }).Where(i => i.PropertyTransactionID == result.ID).ToList()
            };
        }
        public async Task<EamisPropertyTransactionDTO> Update(EamisPropertyTransactionDTO item)
        {
            try
            {
                EAMISPROPERTYTRANSACTION data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
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
                TRANSACTION_TYPE = item.TransactionType,//note: please ensure transaction type is equal to Property Disposal
                MEMO = item.Memo,
                RECEIVED_BY = item.ReceivedBy,
                APPROVED_BY = item.ApprovedBy,
                DELIVERY_DATE = item.DeliveryDate,
                USER_STAMP = item.UserStamp,
                TRANSACTION_STATUS = item.TransactionStatus,
                BRANCH_ID = item.BranchID
            };
        }
        #endregion property transaction
    }

}