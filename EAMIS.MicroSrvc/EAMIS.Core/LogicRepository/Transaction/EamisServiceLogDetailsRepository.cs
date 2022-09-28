using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;

using System;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.LookUp;
using System.Collections.Generic;

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisServiceLogDetailsRepository : IEamisServiceLogDetailsRepository
    {

        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisServiceLogDetailsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        private EAMISSERVICELOGDETAILS MapToEntity(EamisServiceLogDetailsDTO item)
        {
            if (item == null) return new EAMISSERVICELOGDETAILS();
            return new EAMISSERVICELOGDETAILS
            {
                ID = item.ID,
                DUE_DATE = item.DueDate,
                PROPERTY_DESC = item.PropertyDescription,
                PROPERTY_NUMBER = item.PropertyNumber,
                APPRAISAL_INCREMENT = item.AppraisalIncrement,
                APPRAISED_VALUE = item.AppraisedValue,
                AREA_SQM = item.AreaSQM,
                ASSESSED_VALUE = item.AssessedValue,
                ASSET_CONDITION = item.AssetCondition,
                NOTES = item.Notes,
                REAL_ESTATE_TAX_PAYMENT = item.RealEstateTaxPayment,
                RECEIVING_AMOUNT = item.ReceivingAmount,
                RECEIVING_TRAN_ID = item.ReceivingTransactionId,
                SERVICE_DATE = item.ServiceDate, //Get from service log => Tran_Date, programatically assign it's value before calling the Insert/create method
                SERVICE_LOG_ID = item.ServiceLogId, //Get from service log => ID, programatically assign it's value before calling the Insert/create method
                SUPPLIER_ID = item.SupplierId,
                SUPPLIER_DESC = item.SupplierDescription
            };
        }
        public async Task<EamisServiceLogDetailsDTO> InsertServiceLogDetails(EamisServiceLogDetailsDTO item)
        {
            try
            {
                EAMISSERVICELOGDETAILS data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.ID = data.ID;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }

            return item;
        }
        public async Task<EamisServiceLogDetailsDTO> UpdateServiceLogDetails(EamisServiceLogDetailsDTO item)
        {
            try
            {
                EAMISSERVICELOGDETAILS data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }
        public async Task<DataList<EamisServiceLogDetailsDTO>> ListServiceLogDetails(EamisServiceLogDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISSERVICELOGDETAILS> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisServiceLogDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }
        private IQueryable<EamisServiceLogDetailsDTO> QueryToDTO(IQueryable<EAMISSERVICELOGDETAILS> query)
        {
            return query.Select(x => new EamisServiceLogDetailsDTO
            {
                ID = x.ID,
                AppraisalIncrement = x.APPRAISAL_INCREMENT,
                AppraisedValue = x.APPRAISED_VALUE,
                AreaSQM = x.AREA_SQM,
                AssessedValue = x.ASSESSED_VALUE,
                AssetCondition = x.ASSET_CONDITION,
                DueDate = x.DUE_DATE,
                Notes = x.NOTES,
                PropertyDescription = x.PROPERTY_DESC,
                PropertyNumber = x.PROPERTY_NUMBER,
                RealEstateTaxPayment = x.REAL_ESTATE_TAX_PAYMENT,
                ReceivingAmount = x.RECEIVING_AMOUNT,
                ReceivingTransactionId = x.RECEIVING_TRAN_ID,
                ReceivingGroup = new EamisPropertyTransactionDTO
                {
                    Id = x.RECEIVING_GROUP.ID,
                    TransactionNumber = x.RECEIVING_GROUP.TRANSACTION_NUMBER
                },

                ServiceDate = x.SERVICE_DATE,
                ServiceLogId = x.SERVICE_LOG_ID,
                ServiceLogGroup = new EamisServiceLogDTO
                {
                    Id = x.SERVICE_LOG_GROUP.ID,
                    ServiceLogType = x.SERVICE_LOG_GROUP.SERVICE_LOG_TYPE,
                    TransactionId = x.SERVICE_LOG_GROUP.TRAN_ID
                },

                SupplierDescription = x.SUPPLIER_DESC,
                SupplierId = x.SUPPLIER_ID,
                SupplierGroup = new EamisSupplierDTO
                {
                    Id = x.SUPPLIER_GROUP.ID,
                    CompanyName = x.SUPPLIER_GROUP.COMPANY_NAME,
                    CompanyDescription = x.SUPPLIER_GROUP.COMPANY_DESCRIPTION,
                    ContactPersonName = x.SUPPLIER_GROUP.CONTACT_PERSON_NAME,
                    ContactPersonNumber = x.SUPPLIER_GROUP.CONTACT_PERSON_NUMBER,
                    AccountName = x.SUPPLIER_GROUP.ACCOUNT_NAME,
                    AccountNumber = x.SUPPLIER_GROUP.ACCOUNT_NUMBER
                }
            });
        }
        private IQueryable<EAMISSERVICELOGDETAILS> PagedQuery(IQueryable<EAMISSERVICELOGDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EAMISSERVICELOGDETAILS> FilteredEntites(EamisServiceLogDetailsDTO filter, IQueryable<EAMISSERVICELOGDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSERVICELOGDETAILS>(true);
            if (filter.ID != 0)
                predicate = predicate.And(x => x.ID == filter.ID);

            if (string.IsNullOrEmpty(filter.PropertyNumber) && filter.PropertyNumber != null)
                predicate = predicate.And(x => x.PROPERTY_NUMBER == filter.PropertyNumber);

            if (string.IsNullOrEmpty(filter.PropertyDescription) && filter.PropertyDescription != null)
                predicate = predicate.And(x => x.PROPERTY_DESC == filter.PropertyDescription);

            var query = custom_query ?? _ctx.EAMIS_SERVICE_LOG_DETAILS;
            return query.Where(predicate);
        }

        #region for creation of new service log
        private IQueryable<EAMISSERVICELOGDETAILS> FilteredEntitiesServiceLogDetailsForCreation(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISSERVICELOGDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSERVICELOGDETAILS>(true);
            //get items under service logs and exclude it from the list
            var arrservicelogs = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking()
                                                               .Where(pn => !(pn.PROPERTY_NUMBER == null || pn.PROPERTY_NUMBER.Trim() == string.Empty))
                                                               .Select(x => x.PROPERTY_NUMBER)
                                                               .ToList();
            //var arrservicelogs = new List<int> { 217, 218, 219 };
            var query = custom_query ??
                        _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Join(_ctx.EAMIS_DELIVERY_RECEIPT,
                                               d1 => d1.d.DR,
                                               dr => dr.TRANSACTION_TYPE,
                                               (d1, dr) => new { d1, dr })
                                        .Where(x => x.d1.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving)
                                        .Select(x => new EAMISSERVICELOGDETAILS
                                        {
                                            ID = 0,
                                            RECEIVING_TRAN_ID = x.d1.d.PROPERTY_TRANS_ID,
                                            PROPERTY_NUMBER = x.d1.d.PROPERTY_NUMBER,
                                            PROPERTY_DESC = x.d1.d.ITEM_DESCRIPTION,
                                            SUPPLIER_ID = x.dr.SUPPLIER_ID,
                                            SUPPLIER_DESC = x.dr.SUPPLIER_GROUP.COMPANY_NAME,
                                            AREA_SQM = x.d1.d.AREA
                                        }).Where(x => !arrservicelogs.Contains(x.PROPERTY_NUMBER));


            return query.Where(predicate);
        }

        public int GetSupplierId(string transactionType)
        {
            var result = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().FirstOrDefault(x => x.TRANSACTION_TYPE == transactionType);
            if (result != null)
                return result.SUPPLIER_ID;
            return 0;
        }

        public string GetSupplierCompany(int supplierId)
        {
            var result = _ctx.EAMIS_SUPPLIER.AsNoTracking().FirstOrDefault(x => x.ID == supplierId);
            if (result != null)
                return result.COMPANY_NAME;
            return string.Empty;
        }

        private IQueryable<EamisServiceLogDetailsCreationDTO> QueryToDTONew(IQueryable<EAMISSERVICELOGDETAILS> query)
        {
            try
            {
                return query.Select(x => new EamisServiceLogDetailsCreationDTO
                {
                    ID = x.ID,
                    AppraisalIncrement = x.APPRAISAL_INCREMENT, //value must be assigned manually by user
                    AppraisedValue = x.APPRAISED_VALUE, //value must be assigned manually by user
                    AreaSQM = x.AREA_SQM,
                    AssessedValue = x.ASSESSED_VALUE, //value must be assigned manually by user
                    AssetCondition = x.ASSET_CONDITION, //value must be assigned manually by user, get the list from Asset condition type
                    //DueDate = x.DUE_DATE, //value must be assigned manually by user
                    //ServiceDate = x.SERVICE_DATE, //value must be assigned manually by user
                    Notes = x.NOTES, //value must be assigned manually by user
                    PropertyDescription = x.PROPERTY_DESC,
                    PropertyNumber = x.PROPERTY_NUMBER,
                    RealEstateTaxPayment = x.REAL_ESTATE_TAX_PAYMENT, //value must be assigned manually by user
                    ReceivingAmount = x.RECEIVING_AMOUNT, //value must be assigned manually by user
                    ReceivingTransactionId = x.RECEIVING_TRAN_ID,
                    //ServiceDate = x.SERVICE_DATE, //Get from service log => Tran_Date, programatically assign it's value before calling the Insert/create method
                    //ServiceLogId = x.SERVICE_LOG_ID, //Get from service log => ID, programatically assign it's value before calling the Insert/create method
                    SupplierId = x.SUPPLIER_ID, //Get from Delivery Receipt => Supplier_ID
                    SupplierDescription = x.SUPPLIER_DESC //Get from Supplier => Company_Name
                });
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                return null;
            }

        }

        public async Task<List<EamisAssetConditionTypeDTO>> GetAssetCondition()
        {
            var result = await Task.Run(() => _ctx.EAMIS_ASSET_CONDITION_TYPE.AsNoTracking().Select(t =>
                        new EamisAssetConditionTypeDTO
                        {
                            Id = t.ID,
                            AssetConditionType = t.ASSET_CONDITION_DESC
                        }).ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EamisTranTypeDTO>> GetTranTypeList()
        {
            var result = await Task.Run(() => _ctx.EAMIS_TRAN_TYPE.AsNoTracking().Select(t =>
                    new EamisTranTypeDTO
                    {
                        Id = t.ID,
                        TranDesc = t.TRAN_DESC,
                        TranType = t.TRAN_TYPE
                    }).ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<DataList<EamisServiceLogDetailsCreationDTO>> ListServiceLogDetailsForCreation(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISSERVICELOGDETAILS> query = FilteredEntitiesServiceLogDetailsForCreation(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisServiceLogDetailsCreationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTONew(paged).ToListAsync()
            };

        }
        #endregion for creation of new service log
        public async Task<string> GetGroupByLand(string itemCode)
        {
            string retValue = "";
            int partialValue = 0;

            var result = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.Where(s => s.PROPERTY_NO == itemCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                partialValue = result[0].CATEGORY_ID;
                var result1 = await Task.Run(() => _ctx.EAMIS_ITEM_CATEGORY.Where(c => c.ID == partialValue).AsNoTracking().ToList()).ConfigureAwait(false);
               
            }
            return retValue;
        }
    }
}