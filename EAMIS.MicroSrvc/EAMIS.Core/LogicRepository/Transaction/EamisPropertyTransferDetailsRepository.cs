using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
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
    public class EamisPropertyTransferDetailsRepository : IEamisPropertyTransferDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly AISContext _actx;
        private readonly int _maxPageSize;
        public EamisPropertyTransferDetailsRepository(EAMISContext ctx, AISContext actx)
        {
            _ctx = ctx;
            _actx = actx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

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
                ITEM_CODE = item.ItemCode,
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
                PROPERTY_CONDITION = item.PropertyCondition,
                REFERENCE_ID = item.transactionDetailId

            };
        }
        #endregion Property transaction details

        #region property items for transfer
        public async Task<DataList<EamisPropertyTransferDetailsDTO>> ListItemsForTranser(EamisPropertyTransferDetailsDTO filter, string tranType, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredItemsForTranserDetails(filter,tranType);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedDetailsQuery(query, resolved_size, resolved_index);

            var result = new DataList<EamisPropertyTransferDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryDetailsToDTOForTranserDetails(paged).ToListAsync(),

            };

            foreach (var item in result.Items)
            {
                item.AssigneeCustodianName = _actx.Personnel.AsNoTracking().Where(p => p.Id == item.AssigneeCustodian).Select(x => x.FirstName + ' ' + x.LastName).FirstOrDefault();
            }
            return result;
        }
        public async Task<DataList<EamisPropertyTransferDetailsDTO>> SearchIssuance(string type, string searchValue, int assigneeCustodian)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = null;
            if (type == "Item Code")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.ITEM_CODE.Contains(searchValue) && x.ASSIGNEE_CUSTODIAN == assigneeCustodian).AsQueryable();
            }
            else if (type == "Item Description")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.ITEM_DESCRIPTION.Contains(searchValue) && x.ASSIGNEE_CUSTODIAN == assigneeCustodian).AsQueryable();
            }
            else if (type == "Transaction Number")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue) && x.ASSIGNEE_CUSTODIAN == assigneeCustodian).AsQueryable();
            }
            else if (type == "Serial Number")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.SERIAL_NUMBER.Contains(searchValue) && x.ASSIGNEE_CUSTODIAN == assigneeCustodian).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue) && x.ASSIGNEE_CUSTODIAN == assigneeCustodian).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            var result = new DataList<EamisPropertyTransferDetailsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryDetailsToDTOForTranserDetails(paged).ToListAsync()
            };
            foreach (var item in result.Items)
            {
                item.AssigneeCustodianName = _actx.Personnel.AsNoTracking().Where(p => p.Id == item.AssigneeCustodian).Select(x => x.FirstName + ' ' + x.LastName).FirstOrDefault();
            }
            return result;
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryForSearch(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query;
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredItemsForTranserDetails(EamisPropertyTransferDetailsDTO filter, string tranType, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);

            var arrDistinctDetailID = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(pn => !(pn.PROPERTY_NUMBER == null || pn.PROPERTY_NUMBER.Trim() == string.Empty))
                                          .GroupBy(x => x.PROPERTY_NUMBER)
                                          .Select(i => i.Max(x => x.ID))
                                          .ToList();
            if (tranType == "PTR") { 
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                            d => d.PROPERTY_TRANS_ID,
                                            h => h.ID,
                                            (d, h) => new { d, h })
                                            .Where(x => arrDistinctDetailID.Contains(x.d.ID) &&
                                                   (x.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance ||
                                                    x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer )
                                                    && x.d.ASSIGNEE_CUSTODIAN == filter.AssigneeCustodian
                                                    && x.d.UNIT_COST >= 50000
                                                   //&& x.h.TRANSACTION_STATUS == PropertyItemStatus.Approved --> to do: uncomment this line to get property items with Approved status only
                                                   )
                                            .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                            {
                                                ID = x.d.ID,
                                                PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
                                                IS_DEPRECIATION = x.d.IS_DEPRECIATION,
                                                DR = x.d.DR,
                                                PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
                                                ITEM_CODE = x.d.ITEM_CODE,
                                                ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
                                                SERIAL_NUMBER = x.d.SERIAL_NUMBER,
                                                PO = x.d.PO,
                                                PR = x.d.PR,
                                                ACQUISITION_DATE = x.d.ACQUISITION_DATE,
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
                                                PROPERTY_CONDITION = x.d.PROPERTY_CONDITION,
                                            });

            //if (filter.PropertyTransactionID != 0)
            //    predicate = predicate.And(x => x.PROPERTY_TRANS_ID == filter.PropertyTransactionID);
            //var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS;
            return query.Where(predicate);
            }
            if (tranType == "ITR")
            {
                var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                           .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                           d => d.PROPERTY_TRANS_ID,
                                           h => h.ID,
                                           (d, h) => new { d, h })
                                           .Where(x => arrDistinctDetailID.Contains(x.d.ID) &&
                                                  (x.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance ||
                                                   x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer)
                                                   && x.d.ASSIGNEE_CUSTODIAN == filter.AssigneeCustodian
                                                   && x.d.UNIT_COST < 50000
                                                  //&& x.h.TRANSACTION_STATUS == PropertyItemStatus.Approved --> to do: uncomment this line to get property items with Approved status only
                                                  )
                                           .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                           {
                                               ID = x.d.ID,
                                               PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
                                               IS_DEPRECIATION = x.d.IS_DEPRECIATION,
                                               DR = x.d.DR,
                                               PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
                                               ITEM_CODE = x.d.ITEM_CODE,
                                               ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
                                               SERIAL_NUMBER = x.d.SERIAL_NUMBER,
                                               PO = x.d.PO,
                                               PR = x.d.PR,
                                               ACQUISITION_DATE = x.d.ACQUISITION_DATE,
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
                                               PROPERTY_CONDITION = x.d.PROPERTY_CONDITION,
                                           });

                //if (filter.PropertyTransactionID != 0)
                //    predicate = predicate.And(x => x.PROPERTY_TRANS_ID == filter.PropertyTransactionID);
                //var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS;
                return query.Where(predicate);
            }
            return null;
        }

        private IQueryable<EamisPropertyTransferDetailsDTO> QueryDetailsToDTOForTranserDetails(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransferDetailsDTO
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
                AssigneeCustodianName = string.Empty,
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
                NewAssigneeCustodian = 0,
                NewResponsibilityCode = string.Empty,
                //TransactionNumber = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(h => h.ID == x.PROPERTY_TRANS_ID).Select(t => t.TRANSACTION_NUMBER).FirstOrDefault(),
                //TransactionType = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(h => h.ID == x.PROPERTY_TRANS_ID).Select(t => t.TRANSACTION_TYPE).FirstOrDefault(),
                PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Where(h => h.ID == x.PROPERTY_TRANS_ID)
                                            .Select(h => new EamisPropertyTransactionDTO
                                            {
                                                Id = h.ID,
                                                TransactionNumber = h.TRANSACTION_NUMBER,
                                                TransactionDate = h.TRANSACTION_DATE,
                                                FiscalPeriod = h.FISCALPERIOD,
                                                TransactionType = h.TRANSACTION_TYPE,
                                                Memo = h.MEMO,
                                                ReceivedBy = h.RECEIVED_BY,
                                                ApprovedBy = h.APPROVED_BY,
                                                DeliveryDate = h.DELIVERY_DATE,
                                                UserStamp = h.USER_STAMP,
                                                TimeStamp = h.TIMESTAMP,
                                                TransactionStatus = h.TRANSACTION_STATUS,
                                                FundSource = h.FUND_SOURCE
                                            }).FirstOrDefault()
            });
        }
        #endregion property items for transfer
        public async Task<string> GetResponsibilityCenterByID(string newResponsibilityCode)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.Where(s => s.RESPONSIBILITY_CENTER == newResponsibilityCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].OFFICE_DESC.ToString() + ":" +
                           result[0].UNIT_DESC.ToString();
            }
            return retValue;
        }
        public async Task<EamisPropertyTransferDetailsDTO> Update(EamisPropertyTransferDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);

            _ctx.Entry(data).State = data.ID == 0 ? EntityState.Added : EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}