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
    public class EamisPropertyTransactionRepository : IEamisPropertyTransactionRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisPropertyTransactionRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
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
                TRANSACTION_DATE = item.TransactionDate,
                FISCALPERIOD = item.FiscalPeriod,
                TRANSACTION_TYPE = item.TransactionType,
                MEMO = item.Memo,
                RECEIVED_BY = item.ReceivedBy,
                APPROVED_BY = item.ApprovedBy,
                DELIVERY_DATE = item.DeliveryDate,
                USER_STAMP = item.UserStamp,
                TIMESTAMP = item.TimeStamp,
                TRANSACTION_STATUS = item.TransactionStatus

                





            };
        }

        public async Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
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
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
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
    }

}
