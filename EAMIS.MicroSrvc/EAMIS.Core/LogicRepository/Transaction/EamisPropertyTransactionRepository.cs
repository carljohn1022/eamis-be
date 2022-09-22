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

        public async Task<DataList<EamisPropertyTransactionDTO>> SearchReceivingforIssuance(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYTRANSACTION> query = null;
            if (type == "Transaction #")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Received by")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.RECEIVED_BY.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(x => x.TRANSACTION_TYPE == TransactionTypeSettings.Issuance && x.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
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
                TransactionStatus = x.TRANSACTION_STATUS

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
            if (!string.IsNullOrEmpty(filter.TransactionType)) predicate = (strict)
                    ? predicate.And(x => x.TRANSACTION_TYPE.ToLower() == filter.TransactionType.ToLower())
                    : predicate.And(x => x.TRANSACTION_TYPE.Contains(filter.TransactionType.ToLower()));
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
            };
        }

    }


}
