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
    public class EamisPropertyTransferRepository : IEamisPropertyTransferRepository
    {
        private readonly EAMISContext _ctx;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        private readonly int _maxPageSize;
        public EamisPropertyTransferRepository(EAMISContext ctx, IEAMISIDProvider EAMISIDProvider)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        #region property transaction
        public async Task<string> GetNextSequenceNumber()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumber(TransactionTypeSettings.PropertyTransfer);
            return nextId;
        }
        public async Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config)
        {
            //_db.Products.FromSqlRaw<Product>(sql, parms.ToArray()).ToList();
            IQueryable<EAMISPROPERTYTRANSACTION> query = FilteredEntites(filter);

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
        private IQueryable<EAMISPROPERTYTRANSACTION> FilteredEntites(EamisPropertyTransactionDTO filter, IQueryable<EAMISPROPERTYTRANSACTION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTION>(true);

            var propertyItemTransfer = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                        d => d.PROPERTY_TRANS_ID,
                                        h => h.ID,
                                        (d, h) => new { d, h })
                                        .Where(x => x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer)
                                        .Select(y => y.d.PROPERTY_TRANS_ID)
                                        .ToList();
            var propertyItemIssuance = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                        d => d.PROPERTY_TRANS_ID,
                                        h => h.ID,
                                        (d, h) => new { d, h })
                                        .Where(x => x.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance &&
                                               !propertyItemTransfer.Contains(x.d.PROPERTY_TRANS_ID))
                                        .Select(y => y.d.PROPERTY_TRANS_ID)
                                        .ToList();

            var ids = propertyItemTransfer.AsEnumerable().Union(propertyItemIssuance);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION.Where(x => ids.Contains(x.ID));
            return query.Where(predicate);
        }
        private IQueryable<EAMISPROPERTYTRANSACTION> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
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
                TransactionStatus = x.TRANSACTION_STATUS,
                PropertyTransactionDetails = x.PROPERTY_TRANSACTION_DETAILS.Select
                                (d => new EamisPropertyTransactionDetailsDTO
                                {
                                    Id = d.ID,
                                    PropertyTransactionID = d.PROPERTY_TRANS_ID,
                                    isDepreciation = d.IS_DEPRECIATION,
                                    Dr = d.DR,
                                    PropertyNumber = d.PROPERTY_NUMBER,
                                    ItemDescription = d.ITEM_DESCRIPTION,
                                    SerialNumber = d.SERIAL_NUMBER,
                                    Po = d.PO,
                                    Pr = d.PR,
                                    AcquisitionDate = d.ACQUISITION_DATE,
                                    AssigneeCustodian = d.ASSIGNEE_CUSTODIAN,
                                    RequestedBy = d.REQUESTED_BY,
                                    Office = d.OFFICE,
                                    Department = d.DEPARTMENT,
                                    ResponsibilityCode = d.RESPONSIBILITY_CODE,
                                    UnitCost = d.UNIT_COST,
                                    Qty = d.QTY,
                                    SalvageValue = d.SALVAGE_VALUE,
                                    BookValue = d.BOOK_VALUE,
                                    EstLife = d.ESTIMATED_LIFE,
                                    Area = d.AREA,
                                    Semi = d.SEMI_EXPANDABLE_AMOUNT,
                                    UserStamp = d.USER_STAMP,
                                    TimeStamp = d.TIME_STAMP,
                                    WarrantyExpiry = d.WARRANTY_EXPIRY,
                                    Invoice = d.INVOICE,
                                    PropertyCondition = d.PROPERTY_CONDITION,
                                }).ToList()
            });
        }
        public async Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            //ensure that recently added record has the correct transaction type number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.PTPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

            //check if the forecasted transaction type matches with the actual transaction type (saved/created in DB)
            //forecasted transaction type = item.TransactionType
            //actual transaction type = item.TransactionType.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0')
            if (item.TransactionNumber != _drType)
            {
                item.TransactionNumber = _drType; //if not matched, replace value of FTT with  ATT

                //reset context state to avoid error
                _ctx.Entry(data).State = EntityState.Detached;

                //call the update method, force to update the transaction type in the DB
                await this.Update(item);
            }
            return item;
        }
        public async Task<EamisPropertyTransactionDTO> Update(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
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
        #endregion property transaction


    }
}