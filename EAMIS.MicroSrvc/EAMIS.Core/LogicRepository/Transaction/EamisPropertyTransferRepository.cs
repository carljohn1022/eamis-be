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

            string strQuery = "SELECT * FROM [dbo].[EAMIS_PROPERTY_TRANSACTION] " +
                              "WHERE ID IN(" +
                              "SELECT D.PROPERTY_TRANS_ID FROM[dbo].[EAMIS_PROPERTY_TRANSACTION_DETAILS] D " +
                              "INNER JOIN[dbo].[EAMIS_PROPERTY_TRANSACTION] H " +
                              "ON D.PROPERTY_TRANS_ID = H.ID " +
                              "WHERE H.TRANSACTION_TYPE IN('Property Transfer') " +
                              "UNION " +
                              "SELECT D.PROPERTY_TRANS_ID FROM[dbo].[EAMIS_PROPERTY_TRANSACTION_DETAILS] D " +
                              "INNER JOIN[dbo].[EAMIS_PROPERTY_TRANSACTION] H " +
                              "ON D.PROPERTY_TRANS_ID = H.ID " +
                              "WHERE H.TRANSACTION_TYPE IN('Property Issuance') AND " +
                              "D.PROPERTY_NUMBER NOT IN " +
                              "(SELECT D.PROPERTY_NUMBER FROM[dbo].[EAMIS_PROPERTY_TRANSACTION_DETAILS] D " +
                              "INNER JOIN[dbo].[EAMIS_PROPERTY_TRANSACTION] H " +
                              "ON D.PROPERTY_TRANS_ID = H.ID " +
                              "WHERE H.TRANSACTION_TYPE IN('Property Transfer')))";
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION.FromSqlRaw(strQuery);
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
                TransactionStatus = x.TRANSACTION_STATUS

            });
        }
        public async Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            //ensure that recently added record has the correct transaction type number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.PTPrefix + Convert.ToString(data.ID).PadLeft(6, '0');

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

        #region Property transaction details 
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredDetails(EamisPropertyTransferDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            if (filter.PropertyTransactionID != 0)
                predicate = predicate.And(x => x.PROPERTY_TRANS_ID == filter.PropertyTransactionID);
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS;
            return query.Where(predicate);
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedDetailsQuery(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<DataList<EamisPropertyTransferDetailsDTO>> List(EamisPropertyTransferDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredDetails(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedDetailsQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyTransferDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryDetailsToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EamisPropertyTransferDetailsDTO> QueryDetailsToDTO(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransferDetailsDTO
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
                PropertyCondition = x.PROPERTY_CONDITION
            });
        }
        public async Task<EamisPropertyTransferDetailsDTO> Insert(EamisPropertyTransferDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            try
            {
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }
        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransferDetailsDTO item)
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
                PO = item.Pr,
                PR = item.Pr,
                ACQUISITION_DATE = item.AcquisitionDate,
                ASSIGNEE_CUSTODIAN = item.NewAssigneeCustodian, // item.AssigneeCustodian,
                REQUESTED_BY = item.RequestedBy,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                RESPONSIBILITY_CODE = item.NewResponsibilityCode, // item.ResponsibilityCode,
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
        #endregion Property transaction details
    }
}