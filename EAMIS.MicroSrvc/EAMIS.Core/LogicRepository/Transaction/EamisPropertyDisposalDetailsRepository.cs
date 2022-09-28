using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
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
    public class EamisPropertyDisposalDetailsRepository : IEamisPropertyDisposalDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyDisposalDetailsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        #region Property transaction details 
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredDetailsForCondemn(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            
            //retrieve all property items with For Condemn service log type
            var arrservicelogs = _ctx.EAMIS_SERVICE_LOG.AsNoTracking()
                                 .Join(_ctx.EAMIS_SERVICE_LOG_DETAILS,
                                       h => h.ID,
                                       d => d.SERVICE_LOG_ID,
                                       (h, d) => new { h, d })
                                 .Where(x => x.h.SERVICE_LOG_TYPE == PropertyItemStatus.ForCondemn)
                                 .Select(x => x.d.RECEIVING_TRAN_ID).ToList();


            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Where(x => x.h.TRANSACTION_TYPE != TransactionTypeSettings.PropertyDisposal)
                                        .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = 0,
                                            ACQUISITION_DATE = x.d.ACQUISITION_DATE,
                                            PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = x.d.IS_DEPRECIATION,
                                            DR = x.d.DR,
                                            PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
                                            ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
                                            ITEM_CODE = x.d.ITEM_CODE,
                                            SERIAL_NUMBER = x.d.SERIAL_NUMBER,
                                            PO = x.d.PO,
                                            PR = x.d.PR,
                                            ASSIGNEE_CUSTODIAN = x.d.ASSIGNEE_CUSTODIAN,
                                            REQUESTED_BY = x.d.REQUESTED_BY,
                                            OFFICE = x.d.OFFICE,
                                            DEPARTMENT = x.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = x.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = x.d.UNIT_COST,
                                            QTY = x.d.QTY,
                                            SALVAGE_VALUE = x.d.SALVAGE_VALUE,
                                            BOOK_VALUE = x.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = x.d.ESTIMATED_LIFE,
                                            AREA = x.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = x.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = x.d.USER_STAMP,
                                            TIME_STAMP = x.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = x.d.WARRANTY_EXPIRY,
                                            INVOICE = x.d.INVOICE,
                                            PROPERTY_CONDITION = x.d.PROPERTY_CONDITION
                                        }).Where(x => arrservicelogs.Contains(x.PROPERTY_TRANS_ID));

            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredDetails(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            if (filter.PropertyTransactionID != 0)
                predicate = predicate.And(x => x.PROPERTY_TRANS_ID == filter.PropertyTransactionID);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Where(x => x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyDisposal)
                                        .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = 0,
                                            ACQUISITION_DATE = x.d.ACQUISITION_DATE,
                                            PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = x.d.IS_DEPRECIATION,
                                            DR = x.d.DR,
                                            PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
                                            ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
                                            ITEM_CODE = x.d.ITEM_CODE,
                                            SERIAL_NUMBER = x.d.SERIAL_NUMBER,
                                            PO = x.d.PO,
                                            PR = x.d.PR,
                                            ASSIGNEE_CUSTODIAN = x.d.ASSIGNEE_CUSTODIAN,
                                            REQUESTED_BY = x.d.REQUESTED_BY,
                                            OFFICE = x.d.OFFICE,
                                            DEPARTMENT = x.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = x.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = x.d.UNIT_COST,
                                            QTY = x.d.QTY,
                                            SALVAGE_VALUE = x.d.SALVAGE_VALUE,
                                            BOOK_VALUE = x.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = x.d.ESTIMATED_LIFE,
                                            AREA = x.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = x.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = x.d.USER_STAMP,
                                            TIME_STAMP = x.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = x.d.WARRANTY_EXPIRY,
                                            INVOICE = x.d.INVOICE,
                                            PROPERTY_CONDITION = x.d.PROPERTY_CONDITION
                                        });
            return query.Where(predicate);
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedDetailsQuery(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> ListForDisposal(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredDetailsForCondemn(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedDetailsQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryDetailsToDTO(paged).ToListAsync(),

            };
        }

        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> List(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredDetails(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedDetailsQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryDetailsToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EamisPropertyTransactionDetailsDTO> QueryDetailsToDTO(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransactionDetailsDTO
            {
                Id = x.ID,
                PropertyTransactionID = x.PROPERTY_TRANS_ID,
                isDepreciation = x.IS_DEPRECIATION,
                Dr = x.DR,
                PropertyNumber = x.PROPERTY_NUMBER,
                ItemDescription = x.ITEM_DESCRIPTION,
                ItemCode = x.ITEM_CODE,
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
                PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(x => new EamisPropertyTransactionDTO
                {
                    Id = x.ID,
                    TransactionStatus = x.TRANSACTION_STATUS,
                    TransactionType = x.TRANSACTION_TYPE,
                    TransactionNumber = x.TRANSACTION_NUMBER,
                    TransactionDate = x.TRANSACTION_DATE,
                    FiscalPeriod = x.FISCALPERIOD,
                    ReceivedBy = x.RECEIVED_BY
                }).Where(i => i.Id == x.PROPERTY_TRANS_ID).FirstOrDefault()
            });
        }
        public async Task<EamisPropertyTransactionDetailsDTO> Insert(EamisPropertyTransactionDetailsDTO item)
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
                _errorMessage = ex.Message;
                bolerror = true;
            }

            return item;
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Update(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            try
            {
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }

            return item;
        }
        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTIONDETAILS();
            return new EAMISPROPERTYTRANSACTIONDETAILS
            {
                ID = item.Id,
                PROPERTY_TRANS_ID = item.PropertyTransactionID, //note: please ensure the newly created property transaction id is assigned to this field
                IS_DEPRECIATION = item.isDepreciation,
                DR = item.Dr,
                PROPERTY_NUMBER = item.PropertyNumber,
                ITEM_DESCRIPTION = item.ItemDescription,
                ITEM_CODE = item.ItemCode,
                SERIAL_NUMBER = item.SerialNumber,
                PO = item.Pr,
                PR = item.Pr,
                ACQUISITION_DATE = item.AcquisitionDate,
                REQUESTED_BY = item.RequestedBy,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
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