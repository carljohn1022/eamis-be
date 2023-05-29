using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
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
    public class EamisPropertyScheduleRepository : IEamisPropertyScheduleRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyScheduleRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<DataList<EamisPropertyScheduleDTO>> Search(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYSCHEDULE> query = null;
            if (type == "Serial Number")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.SERIAL_NO.Contains(searchValue)).AsQueryable();
            }
            if (type == "Item Description")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.ITEM_DESCRIPTION.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Item Category")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.CATEGORY.Contains(searchValue)).AsQueryable();
            }

            //var paged = PagedQueryForSearch(query);
            //return new DataList<EamisPropertyScheduleDTO>
            //{
            //    Count = await paged.CountAsync(),
            //    Items = await QueryToDTO(paged).ToListAsync()
            //};
            var paged = PagedQueryForSearch(query);
            var result = new DataList<EamisPropertyScheduleDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
            //display latest value
            for (int intAsset = 0; intAsset < result.Items.Count(); intAsset++)
            {
                var propertyTransaction = GetPropertyTransactionDetailsLatestValue(result.Items[intAsset].PropertyNumber,
                                                                                   result.Items[intAsset].ItemCode,
                                                                                   result.Items[intAsset].SerialNo);
                var propertyDelivery = GetPropertyItemDeliveryLatestValue(propertyTransaction.Dr);
                var propertyItem = GetPropertyItemLatestValue(propertyTransaction.ItemCode);
                var serviceLogs = GetPropertyItemServiceLogDetails(propertyTransaction.PropertyNumber);

                result.Items[intAsset].Status = propertyTransaction.PropertyTransactionGroup.TransactionStatus;
                result.Items[intAsset].Category = propertyItem.ItemCategory.CategoryName;
                result.Items[intAsset].SubCategory = propertyItem.PropertyName;
                result.Items[intAsset].VendorName = propertyDelivery.Supplier.CompanyDescription;
                result.Items[intAsset].RRDate = propertyDelivery.DRDate;

                result.Items[intAsset].PropertyNumber = propertyTransaction.PropertyNumber;
                result.Items[intAsset].ItemDescription = propertyTransaction.ItemDescription;
                result.Items[intAsset].AreaSQM = propertyTransaction.Area;
                result.Items[intAsset].Location = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking()
                                                      .Where(r => r.RESPONSIBILITY_CENTER == propertyTransaction.ResponsibilityCode)
                                                      .Select(v => v.LOCATION_DESC).FirstOrDefault();
                result.Items[intAsset].Department = propertyTransaction.Department;
                result.Items[intAsset].LastDepartment = GetPropertyItemLastDepartment(result.Items[intAsset].ReferenceId, propertyTransaction.Id);
                result.Items[intAsset].InvoiceNo = propertyTransaction.Invoice;
                result.Items[intAsset].CostCenter = propertyTransaction.ResponsibilityCode;
                result.Items[intAsset].WarrantyDate = propertyTransaction.AcquisitionDate.AddMonths(propertyTransaction.WarrantyExpiry);
                result.Items[intAsset].PORef = propertyTransaction.Po;
                result.Items[intAsset].RRRef = propertyDelivery.DRNumFrSupplier;
                result.Items[intAsset].ItemCode = propertyTransaction.ItemCode;
                result.Items[intAsset].Warranty = propertyTransaction.WarrantyExpiry;

                if (serviceLogs != null)
                {
                    result.Items[intAsset].AssetCondition = serviceLogs.AssetCondition;
                    result.Items[intAsset].AppraisedValue = serviceLogs.AppraisedValue;
                    result.Items[intAsset].AssessedValue = serviceLogs.AssessedValue;
                    result.Items[intAsset].DisposedAmount = serviceLogs.AssessedValue;
                    result.Items[intAsset].AppraisalIncrement = serviceLogs.AppraisalIncrement;
                    result.Items[intAsset].RealEstateTaxPayment = serviceLogs.RealEstateTaxPayment;
                }

                result.Items[intAsset].ForDepreciation = propertyItem.ItemCategory.ForDepreciation;
                result.Items[intAsset].RevaluationCost = GetPropertyItemRevaluationLatestValue(propertyTransaction.ItemCode);
            }


            return result;
        }
        private IQueryable<EAMISPROPERTYSCHEDULE> PagedQueryForSearch(IQueryable<EAMISPROPERTYSCHEDULE> query)
        {
            return query;
        }

        private string GetPropertyItemLastDepartment(int propertyItemDetailsId, int maxId)
        {
            var latestId = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                               .Where(i => i.REFERENCE_ID == propertyItemDetailsId && i.ID != maxId)
                               .GroupBy(g => g.ID)
                               .Select(i => i.Max(x => x.ID))
                               .FirstOrDefault();
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                             .Where(i => i.ID == latestId)
                             .Select(v => v.DEPARTMENT)
                             .FirstOrDefault();
            return result;
        }
        private EamisServiceLogDetailsDTO GetPropertyItemServiceLogDetails(string propertyNumber)
        {
            var result = _ctx.EAMIS_SERVICE_LOG_DETAILS
                               .Where(i => i.ID == (_ctx.EAMIS_SERVICE_LOG_DETAILS
                                                        .Where(r => r.PROPERTY_NUMBER == propertyNumber)
                                                        .GroupBy(g => g.ID)
                                                        .Select(i => i.Max(x => x.ID))
                                                    )
                               .FirstOrDefault())
                               .Select(s => new EamisServiceLogDetailsDTO
                               {
                                   ID = s.ID,
                                   AssetCondition = s.ASSET_CONDITION,
                                   AssessedValue = s.ASSESSED_VALUE,
                                   AppraisedValue = s.APPRAISED_VALUE,
                                   AppraisalIncrement = s.APPRAISAL_INCREMENT,
                                   RealEstateTaxPayment = s.REAL_ESTATE_TAX_PAYMENT
                               }).FirstOrDefault();
            return result;
        }
        private decimal GetPropertyItemRevaluationLatestValue(string itemCode)
        {
            var result = _ctx.EAMIS_PROPERTY_REVALUATION_DETAILS
                             .Where(i => i.ID == (_ctx.EAMIS_PROPERTY_REVALUATION_DETAILS
                                                      .Where(i => i.ITEM_CODE == itemCode)
                                                      .GroupBy(g => g.ITEM_CODE)
                                                      .Select(i => i.Max(x => x.ID))
                                                  )
                             .FirstOrDefault())
                             .Select(r => r.REVALUED_AMT).FirstOrDefault();
            return result;
        }

        private EamisDeliveryReceiptDTO GetPropertyItemDeliveryLatestValue(string drNumber)
        {
            var result = _ctx.EAMIS_DELIVERY_RECEIPT
                             .Where(d => d.TRANSACTION_TYPE == drNumber)
                             .Select(d => new EamisDeliveryReceiptDTO
                             {
                                 Id = d.ID,
                                 DRDate = d.DR_BY_SUPPLIER_DATE,
                                 DRNumFrSupplier = d.DR_BY_SUPPLIER_NUMBER,
                                 Supplier = _ctx.EAMIS_SUPPLIER.AsNoTracking().Select(s => new EamisSupplierDTO
                                 {
                                     Id = s.ID,
                                     CompanyDescription = s.COMPANY_DESCRIPTION
                                 }).Where(i => i.Id == d.SUPPLIER_ID).FirstOrDefault()
                             }).FirstOrDefault();
            return result;
        }

        private EamisPropertyItemsDTO GetPropertyItemLatestValue(string itemCode)
        {
            var result = _ctx.EAMIS_PROPERTYITEMS
                             .Where(i => i.PROPERTY_NO == itemCode)
                             .Select(item => new EamisPropertyItemsDTO
                             {
                                 Id = item.ID,
                                 ItemCategory = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().Select(cat => new EamisItemCategoryDTO
                                 {
                                     Id = cat.ID,
                                     CategoryName = cat.CATEGORY_NAME,
                                     ForDepreciation = cat.FOR_DEPRECIATION
                                 }).Where(catId => catId.Id == item.CATEGORY_ID).FirstOrDefault(),
                                 PropertyName = _ctx.EAMIS_ITEMS_SUB_CATEGORY.AsNoTracking()
                                                    .Where(subCatId => subCatId.ID == item.SUBCATEGORY_ID)
                                                    .Select(subCat => subCat.SUB_CATEGORY_NAME).FirstOrDefault()
                             }).FirstOrDefault();
            return result;
        }
        private EamisPropertyTransactionDetailsDTO GetPropertyTransactionDetailsLatestValue(string propertyTransactionId,
                                                                                            string itemCode,
                                                                                            string serialNumber)
        {

            var latestId = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                          .Where(refId => refId.PROPERTY_NUMBER == propertyTransactionId &&
                                                          refId.ITEM_CODE == itemCode &&
                                                          refId.SERIAL_NUMBER == serialNumber)
                                          .GroupBy(x => x.PROPERTY_NUMBER)
                                          .Select(i => i.Max(x => x.ID));
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                             .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                             pd => pd.PROPERTY_TRANS_ID, //Property Transaction Details
                             ph => ph.ID, //Property Transaction 
                             (pd, ph) => new { pd, ph })
                             .Where(i => latestId.Contains(i.pd.ID))
                             .Select(item => new EamisPropertyTransactionDetailsDTO
                             {
                                 Id = item.pd.ID,
                                 PropertyTransactionID = item.pd.PROPERTY_TRANS_ID,
                                 isDepreciation = item.pd.IS_DEPRECIATION,
                                 Dr = item.pd.DR,
                                 PropertyNumber = item.pd.PROPERTY_NUMBER,
                                 ItemCode = item.pd.ITEM_CODE,
                                 ItemDescription = item.pd.ITEM_DESCRIPTION,
                                 SerialNumber = item.pd.SERIAL_NUMBER,
                                 Po = item.pd.PO,
                                 Pr = item.pd.PR,
                                 AcquisitionDate = item.pd.ACQUISITION_DATE,
                                 AssigneeCustodian = item.pd.ASSIGNEE_CUSTODIAN,
                                 RequestedBy = item.pd.REQUESTED_BY,
                                 Office = item.pd.OFFICE,
                                 Department = item.pd.DEPARTMENT,
                                 ResponsibilityCode = item.pd.RESPONSIBILITY_CODE,
                                 UnitCost = item.pd.UNIT_COST,
                                 Qty = item.pd.QTY,
                                 IssuedQty = 0,
                                 RemainingQty = 0,
                                 SalvageValue = item.pd.SALVAGE_VALUE,
                                 BookValue = item.pd.BOOK_VALUE,
                                 EstLife = item.pd.ESTIMATED_LIFE,
                                 Area = item.pd.AREA,
                                 Semi = item.pd.SEMI_EXPANDABLE_AMOUNT,
                                 UserStamp = item.pd.USER_STAMP,
                                 TimeStamp = item.pd.TIME_STAMP,
                                 WarrantyExpiry = item.pd.WARRANTY_EXPIRY,
                                 Invoice = item.pd.INVOICE,
                                 PropertyCondition = item.pd.PROPERTY_CONDITION,
                                 transactionDetailId = item.pd.REFERENCE_ID,
                                 PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(h => new EamisPropertyTransactionDTO
                                 {
                                     Id = h.ID,
                                     TransactionStatus = h.TRANSACTION_STATUS
                                 }).Where(i => i.Id == item.pd.PROPERTY_TRANS_ID).FirstOrDefault()
                             }).FirstOrDefault();
            return result;
        }

        public async Task<DataList<EamisPropertyScheduleDTO>> List(EamisPropertyScheduleDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYSCHEDULE> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            var result = new DataList<EamisPropertyScheduleDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
            //display latest value
            for (int intAsset = 0; intAsset < result.Items.Count(); intAsset++)
            {
                var propertyTransaction = GetPropertyTransactionDetailsLatestValue(result.Items[intAsset].PropertyNumber,
                                                                                   result.Items[intAsset].ItemCode,
                                                                                   result.Items[intAsset].SerialNo);
                var propertyDelivery = GetPropertyItemDeliveryLatestValue(propertyTransaction.Dr);
                var propertyItem = GetPropertyItemLatestValue(propertyTransaction.ItemCode);
                var serviceLogs = GetPropertyItemServiceLogDetails(propertyTransaction.PropertyNumber);

                result.Items[intAsset].Status = propertyTransaction.PropertyTransactionGroup.TransactionStatus;
                result.Items[intAsset].Category = propertyItem.ItemCategory.CategoryName;
                result.Items[intAsset].SubCategory = propertyItem.PropertyName;
                result.Items[intAsset].VendorName = propertyDelivery.Supplier.CompanyDescription;
                result.Items[intAsset].RRDate = propertyDelivery.DRDate;

                result.Items[intAsset].PropertyNumber = propertyTransaction.PropertyNumber;
                result.Items[intAsset].ItemDescription = propertyTransaction.ItemDescription;
                result.Items[intAsset].AreaSQM = propertyTransaction.Area;
                result.Items[intAsset].Location = _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking()
                                                      .Where(r => r.RESPONSIBILITY_CENTER == propertyTransaction.ResponsibilityCode)
                                                      .Select(v => v.LOCATION_DESC).FirstOrDefault();
                result.Items[intAsset].Department = propertyTransaction.Department;
                result.Items[intAsset].LastDepartment = GetPropertyItemLastDepartment(result.Items[intAsset].ReferenceId, propertyTransaction.Id);
                result.Items[intAsset].InvoiceNo = propertyTransaction.Invoice;
                result.Items[intAsset].CostCenter = propertyTransaction.ResponsibilityCode;
                result.Items[intAsset].WarrantyDate = propertyTransaction.AcquisitionDate.AddMonths(propertyTransaction.WarrantyExpiry);
                result.Items[intAsset].PORef = propertyTransaction.Po;
                result.Items[intAsset].RRRef = propertyDelivery.DRNumFrSupplier;
                result.Items[intAsset].ItemCode = propertyTransaction.ItemCode;
                result.Items[intAsset].Warranty = propertyTransaction.WarrantyExpiry;

                if (serviceLogs != null)
                {
                    result.Items[intAsset].AssetCondition = serviceLogs.AssetCondition;
                    result.Items[intAsset].AppraisedValue = serviceLogs.AppraisedValue;
                    result.Items[intAsset].AssessedValue = serviceLogs.AssessedValue;
                    result.Items[intAsset].DisposedAmount = serviceLogs.AssessedValue;
                    result.Items[intAsset].AppraisalIncrement = serviceLogs.AppraisalIncrement;
                    result.Items[intAsset].RealEstateTaxPayment = serviceLogs.RealEstateTaxPayment;
                }

                result.Items[intAsset].ForDepreciation = propertyItem.ItemCategory.ForDepreciation;
                result.Items[intAsset].RevaluationCost = GetPropertyItemRevaluationLatestValue(propertyTransaction.ItemCode);
            }


            return result;
        }


        private IQueryable<EAMISPROPERTYSCHEDULE> FilteredEntites(EamisPropertyScheduleDTO filter, IQueryable<EAMISPROPERTYSCHEDULE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYSCHEDULE>(true);

            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ForDepreciation != null && filter.ForDepreciation != false)
                predicate = predicate.And(x => x.FOR_DEPRECIATION == filter.ForDepreciation);
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                     : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.ItemDescription)) predicate = (strict)
                     ? predicate.And(x => x.ITEM_DESCRIPTION.ToLower() == filter.ItemDescription.ToLower())
                     : predicate.And(x => x.ITEM_DESCRIPTION.Contains(filter.ItemDescription.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNo)) predicate = (strict)
                     ? predicate.And(x => x.SERIAL_NO.ToLower() == filter.SerialNo.ToLower())
                     : predicate.And(x => x.SERIAL_NO.Contains(filter.SerialNo.ToLower()));
            if (filter.AcquisitionDate != null && filter.AcquisitionDate != DateTime.MinValue)
                predicate = predicate.And(x => x.ACQUISITION_DATE == filter.AcquisitionDate);

            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                     ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                     : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));

            if (filter.SalvageValue != null && filter.SalvageValue != 0)
                predicate = predicate.And(x => x.SALVAGE_VALUE == filter.SalvageValue);
            if (filter.BookValue != null && filter.BookValue != 0)
                predicate = predicate.And(x => x.BOOK_VALUE == filter.BookValue);
            if (filter.ESTLife != null && filter.ESTLife != 0)
                predicate = predicate.And(x => x.EST_LIFE == filter.ESTLife);
            if (filter.AreaSQM != null && filter.AreaSQM != 0)
                predicate = predicate.And(x => x.AREA_SQM == filter.AreaSQM);

            //predicate = predicate.And(x => x.ID == 42);
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_SCHEDULE;
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYSCHEDULE> PagedQuery(IQueryable<EAMISPROPERTYSCHEDULE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisPropertyScheduleDTO> QueryToDTO(IQueryable<EAMISPROPERTYSCHEDULE> query)
        {
            return query.Select(d => new EamisPropertyScheduleDTO
            {
                Id = d.ID,
                AcquisitionCost = d.ACQUISITION_COST,
                AcquisitionDate = d.ACQUISITION_DATE,
                AppraisalIncrement = d.APPRAISAL_INCREMENT,
                AppraisedValue = d.APPRAISED_VALUE,
                AreaSQM = d.AREA_SQM,
                AssessedValue = d.ASSESSED_VALUE,
                AssetCondition = d.ASSET_CONDITION,
                AssetTag = d.ASSET_TAG,
                DepreciableCost = d.DEPRECIABLE_COST,  //BookValue = d.BOOK_VALUE,
                BookValue = d.BOOK_VALUE,
                Category = d.CATEGORY,
                CostCenter = d.COST_CENTER,
                Department = d.DEPARTMENT,
                DeprecAmount = d.DEPREC_AMOUNT,
                Details = d.DETAILS,
                DisposedAmount = d.DISPOSED_AMOUNT,
                ESTLife = d.EST_LIFE,
                ForDepreciation = d.FOR_DEPRECIATION,
                InvoiceNo = d.INVOICE_NO,
                ItemDescription = d.ITEM_DESCRIPTION,
                ItemCode = d.ITEM_CODE,
                LastDepartment = d.LAST_DEPARTMENT,
                LastPostedDate = d.LAST_POSTED_DATE,
                Location = d.LOCATION,
                Names = d.NAMES,
                PORef = d.POREF,
                PropertyNumber = d.PROPERTY_NUMBER, //Get latest value from property transaction details
                RealEstateTaxPayment = d.REAL_ESTATE_TAX_PAYMENT,
                RevaluationCost = d.REVALUATION_COST,
                RRDate = d.RRDATE,
                RRRef = d.RRREF,
                SalvageValue = d.SALVAGE_VALUE,
                SerialNo = d.SERIAL_NO,
                Status = d.STATUS, //Get latest value from property transaction
                SubCategory = d.SUB_CATEGORY,  
                SvcAgreementNo = d.SVC_AGREEMENT_NO,
                VendorName = d.VENDORNAME,
                Warranty = d.WARRANTY,
                WarrantyDate = d.WARRANTY_DATE,
                ReferenceId = d.REFERENCE_ID,
                AccumulatedDepreciationAmount = d.ACCUMULATED_DEPREC_AMT,
                RemainingLife = d.REMAINING_LIFE
            });
        }

        public async Task<EamisPropertyScheduleDTO> Update(EamisPropertyScheduleDTO item)
        {
            EAMISPROPERTYSCHEDULE schedule = new EAMISPROPERTYSCHEDULE
            {
                ID = item.Id,
                ACCUMULATED_DEPREC_AMT = item.AccumulatedDepreciationAmount,
                REMAINING_LIFE = item.RemainingLife,
                DEPRECIABLE_COST = item.DepreciableCost, //BOOK_VALUE = item.BookValue,
                BOOK_VALUE = item.BookValue,
                ACQUISITION_COST = item.AcquisitionCost,
                ACQUISITION_DATE = item.AcquisitionDate,
                APPRAISAL_INCREMENT = item.AppraisalIncrement,
                APPRAISED_VALUE = item.AppraisedValue,
                AREA_SQM = item.AreaSQM,
                ASSESSED_VALUE = item.AssessedValue,
                ASSET_CONDITION = item.AssetCondition,
                ASSET_TAG = item.AssetTag,
                CATEGORY = item.Category, //get from category, link to property item
                COST_CENTER = item.CostCenter,
                DEPARTMENT = item.Department,
                DEPREC_AMOUNT = item.DeprecAmount,
                DETAILS = item.Details,
                DISPOSED_AMOUNT = item.DisposedAmount,
                EST_LIFE = item.ESTLife,
                FOR_DEPRECIATION = item.ForDepreciation, //to do: include
                INVOICE_NO = item.InvoiceNo,
                ITEM_DESCRIPTION = item.ItemDescription,
                ITEM_CODE = item.ItemCode,
                LAST_DEPARTMENT = item.LastDepartment,
                LAST_POSTED_DATE = item.LastPostedDate,
                LOCATION = item.Location,
                NAMES = item.Names,
                POREF = item.PORef,
                PROPERTY_NUMBER = item.PropertyNumber,
                REAL_ESTATE_TAX_PAYMENT = item.RealEstateTaxPayment,
                REVALUATION_COST = item.RevaluationCost,
                RRDATE = item.RRDate,
                RRREF = item.RRRef,
                SALVAGE_VALUE = item.SalvageValue, //calculated salvage valued must be included when inserting record to property schedule //salvage value = item.ACQUISITION_COST * salvageValue (85000 * 0.05)
                SERIAL_NO = item.SerialNo,
                STATUS = item.Status,
                SUB_CATEGORY = item.SubCategory, //get from sub category, link to property item/Category
                SVC_AGREEMENT_NO = item.SvcAgreementNo,
                VENDORNAME = item.VendorName,
                WARRANTY = item.Warranty, //item.WarrantyExpiry?
                WARRANTY_DATE = item.WarrantyDate,  //item.WarrantyExpiry?
                REFERENCE_ID = item.ReferenceId
            };
            _ctx.Entry(schedule).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisPropertyScheduleDTO>> ListItemsForRevaluationCreation(EamisPropertyScheduleDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYSCHEDULE> query = FilteredEntitesForRevaluationCreation(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQueryForRevaluationCreation(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyScheduleDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTOForRevaluationCreation(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISPROPERTYSCHEDULE> FilteredEntitesForRevaluationCreation(EamisPropertyScheduleDTO filter, IQueryable<EAMISPROPERTYSCHEDULE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYSCHEDULE>(true);

            if (!string.IsNullOrEmpty(filter.BranchID)) predicate = (strict)
                     ? predicate.And(x => x.BRANCH_ID.ToLower() == filter.BranchID.ToLower())
                     : predicate.And(x => x.BRANCH_ID.Contains(filter.BranchID.ToLower()));

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_SCHEDULE;
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYSCHEDULE> PagedQueryForRevaluationCreation(IQueryable<EAMISPROPERTYSCHEDULE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisPropertyScheduleDTO> QueryToDTOForRevaluationCreation(IQueryable<EAMISPROPERTYSCHEDULE> query)
        {
            return query.Select(d => new EamisPropertyScheduleDTO
            {
                Id = d.ID,
                AcquisitionCost = d.ACQUISITION_COST,
                AcquisitionDate = d.ACQUISITION_DATE,
                AppraisalIncrement = d.APPRAISAL_INCREMENT,
                AppraisedValue = d.APPRAISED_VALUE,
                AreaSQM = d.AREA_SQM,
                AssessedValue = d.ASSESSED_VALUE,
                AssetCondition = d.ASSET_CONDITION,
                AssetTag = d.ASSET_TAG,
                BookValue = d.BOOK_VALUE,
                Category = d.CATEGORY,
                CostCenter = d.COST_CENTER,
                Department = d.DEPARTMENT,
                DeprecAmount = d.DEPREC_AMOUNT,
                Details = d.DETAILS,
                DisposedAmount = d.DISPOSED_AMOUNT,
                ESTLife = d.EST_LIFE,
                ForDepreciation = d.FOR_DEPRECIATION,
                InvoiceNo = d.INVOICE_NO,
                ItemDescription = d.ITEM_DESCRIPTION,
                ItemCode = d.ITEM_CODE,
                LastDepartment = d.LAST_DEPARTMENT,
                LastPostedDate = d.LAST_POSTED_DATE,
                Location = d.LOCATION,
                Names = d.NAMES,
                PORef = d.POREF,
                PropertyNumber = d.PROPERTY_NUMBER,
                RealEstateTaxPayment = d.REAL_ESTATE_TAX_PAYMENT,
                RevaluationCost = d.REVALUATION_COST,
                RRDate = d.RRDATE,
                RRRef = d.RRREF,
                SalvageValue = d.SALVAGE_VALUE,
                SerialNo = d.SERIAL_NO,
                Status = d.STATUS,
                SubCategory = d.SUB_CATEGORY,
                SvcAgreementNo = d.SVC_AGREEMENT_NO,
                VendorName = d.VENDORNAME,
                Warranty = d.WARRANTY,
                WarrantyDate = d.WARRANTY_DATE
            });
        }

        public async Task<string> GetEstimatedLife(string itemCode)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.Where(s => s.PROPERTY_NO == itemCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                var result1 = await Task.Run(() => _ctx.EAMIS_ITEM_CATEGORY.Where(s => s.ID == result[0].CATEGORY_ID).AsNoTracking().ToList()).ConfigureAwait(false);
                retValue = result1[0].ESTIMATED_LIFE.ToString();
            }
            return retValue;
        }
    }
}
