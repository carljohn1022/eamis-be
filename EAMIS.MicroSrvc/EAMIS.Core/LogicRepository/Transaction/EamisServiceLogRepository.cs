using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using EAMIS.Common.DTO.Masterfiles;

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisServiceLogRepository : IEamisServiceLogRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        public EamisServiceLogRepository(EAMISContext ctx,
                                               IEAMISIDProvider EAMISIDProvider)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<string> GetNextSequenceNumber()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumber(TransactionTypeSettings.ServiceLog);
            return nextId;
        }

        public async Task<DataList<EamisServiceLogDTO>> ListServiceLogs(EamisServiceLogDTO filter, PageConfig config)
        {
            IQueryable<EAMISSERVICELOG> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisServiceLogDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EamisServiceLogDTO> QueryToDTO(IQueryable<EAMISSERVICELOG> query)
        {
            var category = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToList();
            return query.Select(x => new EamisServiceLogDTO
            {
                Id = x.ID,
                ServiceLogType = x.SERVICE_LOG_TYPE,
                TransactionId = x.TRAN_ID,
                TransactionDate = x.TRAN_DATE,
                ServiceLogDetails = x.SERVICE_LOG_DETAILS.Select
                                    (d => new EamisServiceLogDetailsDTO
                                    {
                                        ID = d.ID,
                                        AppraisalIncrement = d.APPRAISAL_INCREMENT,
                                        AppraisedValue = d.APPRAISED_VALUE,
                                        AreaSQM = d.AREA_SQM,
                                        AssessedValue = d.ASSESSED_VALUE,
                                        AssetCondition = d.ASSET_CONDITION,
                                        DueDate = d.DUE_DATE,
                                        Notes = d.NOTES,
                                        PropertyDescription = d.PROPERTY_DESC,
                                        PropertyNumber = d.PROPERTY_NUMBER,
                                        RealEstateTaxPayment = d.REAL_ESTATE_TAX_PAYMENT,
                                        ReceivingAmount = d.RECEIVING_AMOUNT,
                                        ReceivingTransactionId = d.RECEIVING_TRAN_ID,
                                        ReceivingGroup = new EamisPropertyTransactionDTO
                                        {
                                            Id = d.RECEIVING_GROUP.ID,
                                            TransactionNumber = d.RECEIVING_GROUP.TRANSACTION_NUMBER
                                        },

                                        ServiceDate = d.SERVICE_DATE,
                                        ServiceLogId = d.SERVICE_LOG_ID,
                                        ServiceLogGroup = new EamisServiceLogDTO
                                        {
                                            Id = d.SERVICE_LOG_GROUP.ID,
                                            ServiceLogType = d.SERVICE_LOG_GROUP.SERVICE_LOG_TYPE,
                                            TransactionId = d.SERVICE_LOG_GROUP.TRAN_ID
                                        },

                                        SupplierDescription = d.SUPPLIER_DESC,
                                        SupplierId = d.SUPPLIER_ID,
                                        SupplierGroup = new EamisSupplierDTO
                                        {
                                            Id = d.SUPPLIER_GROUP.ID,
                                            CompanyName = d.SUPPLIER_GROUP.COMPANY_NAME,
                                            CompanyDescription = d.SUPPLIER_GROUP.COMPANY_DESCRIPTION,
                                            ContactPersonName = d.SUPPLIER_GROUP.CONTACT_PERSON_NAME,
                                            ContactPersonNumber = d.SUPPLIER_GROUP.CONTACT_PERSON_NUMBER,
                                            AccountName = d.SUPPLIER_GROUP.ACCOUNT_NAME,
                                            AccountNumber = d.SUPPLIER_GROUP.ACCOUNT_NUMBER
                                        }
                                    }
                                    ).ToList()
            });
        }

        private IQueryable<EAMISSERVICELOG> PagedQuery(IQueryable<EAMISSERVICELOG> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISSERVICELOG> FilteredEntites(EamisServiceLogDTO filter, IQueryable<EAMISSERVICELOG> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSERVICELOG>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (string.IsNullOrEmpty(filter.TransactionId) && filter.TransactionId != null)
                predicate = predicate.And(x => x.TRAN_ID == filter.TransactionId);

            var query = custom_query ?? _ctx.EAMIS_SERVICE_LOG;
            return query.Where(predicate);
        }

        public async Task<EamisServiceLogDTO> InsertServiceLog(EamisServiceLogDTO item)
        {
            EAMISSERVICELOG data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            //ensure that recently added record has the correct transaction id number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            string _drType = PrefixSettings.SLPrefix + Convert.ToString(data.ID).PadLeft(6, '0');

            //check if the forecasted transaction id matches with the actual transaction id (saved/created in DB)
            //forecasted transaction id = item.TransactionId
            //actual transaction id = item.TransactionId.Substring(0, 6) + Convert.ToString(data.ID).PadLeft(6, '0')
            if (item.TransactionId != _drType)
            {
                item.TransactionId = _drType;

                //reset context state to avoid error
                _ctx.Entry(data).State = EntityState.Detached;

                //call the update method, force to update the transaction id in the DB
                await this.UpdateServiceLog(item);
            }
            return item;
        }

        public async Task<EamisServiceLogDTO> UpdateServiceLog(EamisServiceLogDTO item)
        {
            if (item.Id == 0)
                return item;
            EAMISSERVICELOG data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISSERVICELOG MapToEntity(EamisServiceLogDTO item)
        {
            if (item == null) return new EAMISSERVICELOG();
            return new EAMISSERVICELOG
            {
                ID = item.Id,
                TRAN_DATE = item.TransactionDate,
                TRAN_ID = item.TransactionId,
                SERVICE_LOG_TYPE = item.ServiceLogType
            };
        }
        public async Task<EamisServiceLogDTO> getServiceItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_SERVICE_LOG.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisServiceLogDTO
            {

                Id = result.ID,
                TransactionDate = result.TRAN_DATE,
                TransactionId = result.TRAN_ID,
                ServiceLogType = result.SERVICE_LOG_TYPE,
                ServiceLogDetails = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking().Select(x => new EamisServiceLogDetailsDTO
                {
                    ID = x.ID,
                    ServiceLogId = x.SERVICE_LOG_ID,
                    ReceivingTransactionId = x.RECEIVING_TRAN_ID,
                    PropertyNumber = x.PROPERTY_NUMBER,
                    PropertyDescription = x.PROPERTY_DESC,
                    AssetCondition = x.ASSET_CONDITION,
                    ReceivingAmount = x.RECEIVING_AMOUNT,
                    SupplierId = x.SUPPLIER_ID,
                    SupplierDescription = x.SUPPLIER_DESC,
                    ServiceDate = x.SERVICE_DATE,
                    DueDate = x.DUE_DATE,
                    AssessedValue = x.ASSESSED_VALUE,
                    AppraisedValue = x.APPRAISED_VALUE,
                    AppraisalIncrement = x.APPRAISAL_INCREMENT,
                    RealEstateTaxPayment = x.REAL_ESTATE_TAX_PAYMENT,
                    AreaSQM = x.AREA_SQM,
                    Notes = x.NOTES
                }).Where(i => i.ServiceLogId == result.ID).ToList()
            };
        }
        //public async Task<EamisServiceLogDTO> getServiceLogItemById(int itemID)
        //{
        //    var result = await Task.Run(() => _ctx.EAMIS_SERVICE_LOG.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
        //    return new EamisServiceLogDTO
        //    {
        //        Id = result.ID,
        //        TransactionDate = result.TRAN_DATE,
        //        TransactionId = result.TRAN_ID,
        //        ServiceLogDetails = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking().Select(w => new EamisServiceLogDetailsDTO
        //        {
        //            ID = w.ID,
        //            AppraisalIncrement = w.APPRAISAL_INCREMENT,
        //            AppraisedValue = w.APPRAISED_VALUE,
        //            AreaSQM = w.AREA_SQM,
        //            AssessedValue = w.ASSESSED_VALUE,
        //            AssetCondition = w.ASSET_CONDITION,
        //            DueDate = w.DUE_DATE,
        //            Notes = w.NOTES,
        //            PropertyDescription = w.PROPERTY_DESC,
        //            PropertyNumber = w.PROPERTY_NUMBER,
        //            RealEstateTaxPayment = w.REAL_ESTATE_TAX_PAYMENT,
        //            ReceivingAmount = w.RECEIVING_AMOUNT,
        //            ReceivingTransactionId = w.RECEIVING_TRAN_ID,
        //            ReceivingGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(r => new EamisPropertyTransactionDTO
        //            {    
        //                Id = r.ID,
        //                TransactionNumber = r.TRANSACTION_NUMBER
        //            }).FirstOrDefault(x => x.Id == w.RECEIVING_TRAN_ID),
        //            ServiceDate = w.SERVICE_DATE,
        //            SupplierDescription = w.SUPPLIER_DESC,
        //            SupplierId = w.SUPPLIER_ID,
        //            SupplierGroup = _ctx.EAMIS_SUPPLIER.AsNoTracking().Select(s => new EamisSupplierDTO
        //            {
        //                Id = s.ID,
        //                CompanyName = s.COMPANY_NAME,
        //                CompanyDescription = s.COMPANY_DESCRIPTION,
        //                ContactPersonName = s.CONTACT_PERSON_NAME,
        //                ContactPersonNumber = s.CONTACT_PERSON_NUMBER,
        //                AccountName = s.ACCOUNT_NAME,
        //                AccountNumber = s.ACCOUNT_NUMBER
        //            }).FirstOrDefault(x => x.Id == w.SUPPLIER_ID),


        //        }).Where(svcId => svcId.ServiceLogId == result.ID).ToList()
        //    };
        //}

    }
}