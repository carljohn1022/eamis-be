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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyTransactionDetailsRepository : IEamisPropertyTransactionDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly IFactorType _factorType;
        private readonly int _maxPageSize;

        private readonly IEamisPropertyRevalutionRepository _eamisPropertyRevalutionRepository;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }

        private string _categoryName = "";
        private string CategoryName { get => _categoryName; set => value = _categoryName; }

        private string _subCategoryName = "";
        private string SubCategoryName { get => _subCategoryName; set => value = _subCategoryName; }

        private int _estimatedLife;
        private int EstimatedLife { get => _estimatedLife; set => value = _estimatedLife; }

        public EamisPropertyTransactionDetailsRepository(EAMISContext ctx,
            IEamisPropertyRevalutionRepository eamisPropertyRevalutionRepository,
            IFactorType factorType)
        {
            _ctx = ctx;
            _factorType = factorType;
            _eamisPropertyRevalutionRepository = eamisPropertyRevalutionRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }


        private bool IsAssetPropertyItem(string itemCode)
        {
            var result = _ctx.EAMIS_PROPERTYITEMS
                        .Join(_ctx.EAMIS_ITEM_CATEGORY,
                        item => item.CATEGORY_ID,
                        category => category.ID,
                        (item, category) => new { item, category })
                        .Join(_ctx.EAMIS_ITEMS_SUB_CATEGORY,
                        itemSubCategory => itemSubCategory.item.SUBCATEGORY_ID,
                        subCategory => subCategory.ID,
                        (itemSubCategory, subCategory) => new { itemSubCategory, subCategory })
                        .Where(p => p.itemSubCategory.item.PROPERTY_NO == itemCode)
                        .Select(c => new
                        {
                            c.itemSubCategory.category.IS_ASSET,
                            c.itemSubCategory.category.CATEGORY_NAME,
                            c.subCategory.SUB_CATEGORY_NAME,
                            c.itemSubCategory.category.ESTIMATED_LIFE
                        }).FirstOrDefault();
            if (result != null)
            {
                _categoryName = result.CATEGORY_NAME;
                _subCategoryName = result.SUB_CATEGORY_NAME;
                _estimatedLife = result.ESTIMATED_LIFE;
                return result.IS_ASSET;
            }
            return false;
        }

        private EAMISPROPERTYSCHEDULE MapAssetScheduleEntity(EamisPropertyTransactionDetailsDTO item)
        {
            //Construct the asset schedule data
            return new EAMISPROPERTYSCHEDULE
            {
                ID = 0,
                ACQUISITION_COST = item.UnitCost,
                ACQUISITION_DATE = item.AcquisitionDate,
                APPRAISAL_INCREMENT = 0,
                APPRAISED_VALUE = 0,
                AREA_SQM = item.Area,
                ASSESSED_VALUE = 0,
                ASSET_CONDITION = string.Empty,
                ASSET_TAG = string.Empty,
                BOOK_VALUE = item.BookValue,
                CATEGORY = CategoryName, //get from category, link to property item
                COST_CENTER = string.Empty,
                DEPARTMENT = item.Department,
                DEPREC_AMOUNT = 0,
                DETAILS = string.Empty,
                DISPOSED_AMOUNT = 0,
                EST_LIFE = item.EstLife,
                FOR_DEPRECIATION = string.Empty,
                INVOICE_NO = item.Invoice,
                ITEM_DESCRIPTION = item.ItemDescription,
                LAST_DEPARTMENT = string.Empty,
                LAST_POSTED_DATE = DateTime.Now,
                LOCATION = string.Empty,
                NAMES = string.Empty,
                POREF = 0,
                PROPERTY_NUMBER = item.PropertyNumber,
                REAL_ESTATE_TAX_PAYMENT = 0,
                REVALUATION_COST = 0,
                RRDATE = DateTime.Now,
                RRREF = 0,
                SALVAGE_VALUE = item.SalvageValue,
                SERIAL_NO = item.SerialNumber,
                STATUS = string.Empty,
                SUB_CATEGORY = SubCategoryName, //get from sub category, link to property item/Category
                SVC_AGREEMENT_NO = 0,
                VENDORNAME = string.Empty,
                WARRANTY = string.Empty, //item.WarrantyExpiry?
                WARRANTY_DATE = DateTime.Now,  //item.WarrantyExpiry?
                ITEM_CODE = item.ItemCode
            };
        }

        private async Task<EAMISPROPERTYREVALUATION> MapPropertyRevaluation()
        {
            return new EAMISPROPERTYREVALUATION
            {
                ID = 0,
                PARTICULARS = string.Empty,
                TRAN_DATE = DateTime.Now,
                TRAN_ID = await Task.Run(() => _eamisPropertyRevalutionRepository.GetNextSequenceNumber()).ConfigureAwait(false)
            };
        }

        private EAMISPROPERTYREVALUATIONDETAILS MapPropertyRevaluationDetails(int revaluationId, EamisPropertyTransactionDetailsDTO item)
        {
            decimal salvageValue = _factorType.GetFactorTypeValue(FactorTypes.SalvageValue); //Get salvage value factor
            decimal bookValue = item.UnitCost - (item.UnitCost * salvageValue); //Unit Cost * Salvage value factor
            decimal monthlyDepreciation = bookValue / EstimatedLife;
            return new EAMISPROPERTYREVALUATIONDETAILS
            {
                ID = 0,
                ACCUMULATIVE_DEPRECIATION = monthlyDepreciation, //Monthly Depreciation * Estimated Life
                ACQ_COST = item.UnitCost, //to be confirmed, assumption, value is the same with property item unit cost
                DEPRECIATION = item.AcquisitionDate, //to be confirmed, Default Depreciation date is Acquisition date plus the estimated life in months
                FAIR_VALUE = 0, //to be confirmed, Book Value after revaluation
                ITEM_CODE = item.ItemCode,
                ITEM_DESC = item.ItemDescription,
                NET_BOOK_VALUE = bookValue, //to be confirmed
                NEW_DEP = item.AcquisitionDate, //to be confirmed, Default value same as Depreciation Date
                PREV_REVALUATION = string.Empty, //to be confirmed
                REMAINING_LIFE = EstimatedLife, //Get estimated life from category. per conversation with Justin 08.22.2022
                REVALUED_AMT = 0, //to be confirmed
                SALVAGE_VALUE = item.SalvageValue,
                PROPERTY_REVALUATION_ID = revaluationId
            };
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item; ;
        }

        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTIONDETAILS();
            return new EAMISPROPERTYTRANSACTIONDETAILS
            {
                ID = item.Id,
                PROPERTY_TRANS_ID = item.PropertyTransactionID,
                IS_DEPRECIATION = item.isDepreciation,
                DR = item.Dr,
                PROPERTY_NUMBER = item.PropertyNumber,
                ITEM_CODE = item.ItemCode,
                ITEM_DESCRIPTION = item.ItemDescription,
                SERIAL_NUMBER = item.SerialNumber,
                PO = item.Po,
                PR = item.Pr,
                ACQUISITION_DATE = item.AcquisitionDate,
                ASSIGNEE_CUSTODIAN = item.AssigneeCustodian,
                REQUESTED_BY = item.RequestedBy,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                RESPONSIBILITY_CODE = item.ResponsibilityCode,
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

        public async Task<EamisPropertyTransactionDetailsDTO> Insert(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            var transaction = _ctx.Database.BeginTransaction();
            try
            {
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                if (item.isDepreciation)
                    if (IsAssetPropertyItem(item.ItemCode)) //check if property item category is under asset
                    {
                        //insert new record to asset schedule
                        EAMISPROPERTYSCHEDULE asset = MapAssetScheduleEntity(item);
                        _ctx.Entry(asset).State = EntityState.Added;
                        await _ctx.SaveChangesAsync();

                        //removed 08.24.2022 per Justin, no need to create
                        ////insert new record to property revaluation
                        //EAMISPROPERTYREVALUATION revaluation = await MapPropertyRevaluation();
                        //_ctx.Entry(revaluation).State = EntityState.Added;
                        //await _ctx.SaveChangesAsync();

                        ////insert new record to property revaluation details
                        //EAMISPROPERTYREVALUATIONDETAILS revaluationDetails = MapPropertyRevaluationDetails(revaluation.ID, item);
                        //_ctx.Entry(revaluationDetails).State = EntityState.Added;
                        //await _ctx.SaveChangesAsync();
                    }
                transaction.Commit();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
                transaction.Rollback();
            }
            return item;
        }

        private List<IssuedQtyDTO> GetIssuedQtyDTO()
        {
            var itemIssued = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                d => d.PROPERTY_TRANS_ID,
                                h => h.ID,
                                (d, h) => new { d, h })
                                .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance)
                                .GroupBy(x => new { x.d.ITEM_CODE, x.d.PO })
                                .Select(i => new IssuedQtyDTO
                                {
                                    ItemCode = i.Key.ITEM_CODE,
                                    PO = i.Key.PO,
                                    IssuedQty = i.Sum(q => q.d.QTY)
                                })
                                .ToList();
            return itemIssued;
        }

        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> List(EamisPropertyTransactionDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredEntitiesNew(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            var result = new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
            
            return result;
        }

        private IQueryable<EamisPropertyTransactionDetailsDTO> QueryToDTO(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransactionDetailsDTO
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
                IssuedQty = 0,
                RemainingQty = 0,
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
                PropertyTransactionGroup = new EamisPropertyTransactionDTO
                {
                    Id = x.PROPERTY_TRANSACTION_GROUP.ID,
                    TransactionStatus = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_STATUS,
                    TransactionNumber = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER,
                    TransactionDate = x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_DATE,
                    FiscalPeriod = x.PROPERTY_TRANSACTION_GROUP.FISCALPERIOD,
                    ReceivedBy = x.PROPERTY_TRANSACTION_GROUP.RECEIVED_BY
                }

            });
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQuery(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredEntitiesNew(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            //get property items issued
            var itemIssued = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                 .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                 d => d.PROPERTY_TRANS_ID,
                                 h => h.ID,
                                 (d, h) => new { d, h })
                                 .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance)
                                 .GroupBy(x => new { x.d.ITEM_CODE, x.d.PO })
                                 .Select(i => new { i.Key.ITEM_CODE, i.Key.PO, IssuedQty = i.Sum(q => q.d.QTY) })
                                 .ToList();

            //get property items with serial number only
            //var itemsWithSerialNo = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
            //                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
            //                            d => d.PROPERTY_TRANS_ID,
            //                            h => h.ID,
            //                            (d, h) => new { d, h })
            //                            .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving //&& 
            //                                                                                                             //!(t.d.SERIAL_NUMBER == null || t.d.SERIAL_NUMBER.ToString().Trim() == string.Empty)
            //                                  )
            //                            .Select(item => new EAMISPROPERTYTRANSACTIONDETAILS
            //                            {
            //                                ID = item.d.ID,
            //                                PROPERTY_TRANS_ID = item.d.PROPERTY_TRANS_ID,
            //                                IS_DEPRECIATION = item.d.IS_DEPRECIATION,
            //                                DR = item.d.DR,
            //                                PROPERTY_NUMBER = item.d.PROPERTY_NUMBER,
            //                                ITEM_CODE = item.d.ITEM_CODE,
            //                                ITEM_DESCRIPTION = item.d.ITEM_DESCRIPTION,
            //                                SERIAL_NUMBER = item.d.SERIAL_NUMBER,
            //                                PO = item.d.PO,
            //                                PR = item.d.PR,
            //                                ACQUISITION_DATE = item.d.ACQUISITION_DATE,
            //                                ASSIGNEE_CUSTODIAN = item.d.ASSIGNEE_CUSTODIAN,
            //                                REQUESTED_BY = item.d.REQUESTED_BY,
            //                                OFFICE = item.d.OFFICE,
            //                                DEPARTMENT = item.d.DEPARTMENT,
            //                                RESPONSIBILITY_CODE = item.d.RESPONSIBILITY_CODE,
            //                                UNIT_COST = item.d.UNIT_COST,
            //                                QTY = item.d.QTY,
            //                                SALVAGE_VALUE = item.d.SALVAGE_VALUE,
            //                                BOOK_VALUE = item.d.BOOK_VALUE,
            //                                ESTIMATED_LIFE = item.d.ESTIMATED_LIFE,
            //                                AREA = item.d.AREA,
            //                                SEMI_EXPANDABLE_AMOUNT = item.d.SEMI_EXPANDABLE_AMOUNT,
            //                                USER_STAMP = item.d.USER_STAMP,
            //                                TIME_STAMP = item.d.TIME_STAMP,
            //                                WARRANTY_EXPIRY = item.d.WARRANTY_EXPIRY,
            //                                INVOICE = item.d.INVOICE,
            //                                PROPERTY_CONDITION = item.d.PROPERTY_CONDITION
            //                            });

            //////get property items without servial number only
            //var itemsWithoutSerialNo = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
            //                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
            //                            d => d.PROPERTY_TRANS_ID,
            //                            h => h.ID,
            //                            (d, h) => new { d, h })
            //                            .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
            //                                        (t.d.SERIAL_NUMBER == null || t.d.SERIAL_NUMBER.ToString().Trim() == string.Empty))
            //                            .Select(item => new EAMISPROPERTYTRANSACTIONDETAILS
            //                            {
            //                                ID = item.d.ID,
            //                                PROPERTY_TRANS_ID = item.d.PROPERTY_TRANS_ID,
            //                                IS_DEPRECIATION = item.d.IS_DEPRECIATION,
            //                                DR = item.d.DR,
            //                                PROPERTY_NUMBER = item.d.PROPERTY_NUMBER,
            //                                ITEM_CODE = item.d.ITEM_CODE,
            //                                ITEM_DESCRIPTION = item.d.ITEM_DESCRIPTION,
            //                                SERIAL_NUMBER = item.d.SERIAL_NUMBER,
            //                                PO = item.d.PO,
            //                                PR = item.d.PR,
            //                                ACQUISITION_DATE = item.d.ACQUISITION_DATE,
            //                                ASSIGNEE_CUSTODIAN = item.d.ASSIGNEE_CUSTODIAN,
            //                                REQUESTED_BY = item.d.REQUESTED_BY,
            //                                OFFICE = item.d.OFFICE,
            //                                DEPARTMENT = item.d.DEPARTMENT,
            //                                RESPONSIBILITY_CODE = item.d.RESPONSIBILITY_CODE,
            //                                UNIT_COST = item.d.UNIT_COST,
            //                                QTY = item.d.QTY,
            //                                SALVAGE_VALUE = item.d.SALVAGE_VALUE,
            //                                BOOK_VALUE = item.d.BOOK_VALUE,
            //                                ESTIMATED_LIFE = item.d.ESTIMATED_LIFE,
            //                                AREA = item.d.AREA,
            //                                SEMI_EXPANDABLE_AMOUNT = item.d.SEMI_EXPANDABLE_AMOUNT,
            //                                USER_STAMP = item.d.USER_STAMP,
            //                                TIME_STAMP = item.d.TIME_STAMP,
            //                                WARRANTY_EXPIRY = item.d.WARRANTY_EXPIRY,
            //                                INVOICE = item.d.INVOICE,
            //                                PROPERTY_CONDITION = item.d.PROPERTY_CONDITION
            //                            });
            ////merge property items
            //var items = itemsWithSerialNo.AsEnumerable().Union(itemsWithoutSerialNo);
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                        d => d.PROPERTY_TRANS_ID, //detail
                                        h => h.ID, //header
                                        (d, h) => new { d, h })
                                        .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving)
                                        .Select(item => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = item.d.ID,
                                            PROPERTY_TRANS_ID = item.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = item.d.IS_DEPRECIATION,
                                            DR = item.d.DR,
                                            PROPERTY_NUMBER = item.d.PROPERTY_NUMBER,
                                            ITEM_CODE = item.d.ITEM_CODE,
                                            ITEM_DESCRIPTION = item.d.ITEM_DESCRIPTION,
                                            SERIAL_NUMBER = item.d.SERIAL_NUMBER,
                                            PO = item.d.PO,
                                            PR = item.d.PR,
                                            ACQUISITION_DATE = item.d.ACQUISITION_DATE,
                                            ASSIGNEE_CUSTODIAN = item.d.ASSIGNEE_CUSTODIAN,
                                            REQUESTED_BY = item.d.REQUESTED_BY,
                                            OFFICE = item.d.OFFICE,
                                            DEPARTMENT = item.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = item.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = item.d.UNIT_COST,
                                            QTY = item.d.QTY,
                                            SALVAGE_VALUE = item.d.SALVAGE_VALUE,
                                            BOOK_VALUE = item.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = item.d.ESTIMATED_LIFE,
                                            AREA = item.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = item.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = item.d.USER_STAMP,
                                            TIME_STAMP = item.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = item.d.WARRANTY_EXPIRY,
                                            INVOICE = item.d.INVOICE,
                                            PROPERTY_CONDITION = item.d.PROPERTY_CONDITION,
                                            PROPERTY_TRANSACTION_GROUP = new EAMISPROPERTYTRANSACTION
                                            {
                                                ID = item.h.ID,
                                                TRANSACTION_STATUS = item.h.TRANSACTION_STATUS,
                                                TRANSACTION_NUMBER = item.h.TRANSACTION_NUMBER,
                                                TRANSACTION_DATE = item.h.TRANSACTION_DATE,
                                                FISCALPERIOD = item.h.FISCALPERIOD,
                                                RECEIVED_BY = item.h.RECEIVED_BY
                                            }
                                        });
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            return query.Where(predicate);
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredEntities(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.isDepreciation != null && filter.isDepreciation != false)
                predicate = predicate.And(x => x.IS_DEPRECIATION == filter.isDepreciation);
            if (!string.IsNullOrEmpty(filter.Dr)) predicate = (strict)
                     ? predicate.And(x => x.DR.ToLower() == filter.Dr.ToLower())
                     : predicate.And(x => x.DR.Contains(filter.Dr.ToLower()));
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                     : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.ItemDescription)) predicate = (strict)
                     ? predicate.And(x => x.ITEM_DESCRIPTION.ToLower() == filter.ItemDescription.ToLower())
                     : predicate.And(x => x.ITEM_DESCRIPTION.Contains(filter.ItemDescription.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNumber)) predicate = (strict)
                     ? predicate.And(x => x.SERIAL_NUMBER.ToLower() == filter.SerialNumber.ToLower())
                     : predicate.And(x => x.SERIAL_NUMBER.Contains(filter.SerialNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.Po)) predicate = (strict)
                     ? predicate.And(x => x.PO.ToLower() == filter.Po.ToLower())
                     : predicate.And(x => x.PO.Contains(filter.Po.ToLower()));
            if (!string.IsNullOrEmpty(filter.Pr)) predicate = (strict)
                     ? predicate.And(x => x.PR.ToLower() == filter.Pr.ToLower())
                     : predicate.And(x => x.PR.Contains(filter.Pr.ToLower()));
            if (filter.AcquisitionDate != null && filter.AcquisitionDate != DateTime.MinValue)
                predicate = predicate.And(x => x.ACQUISITION_DATE == filter.AcquisitionDate);
            if (filter.AssigneeCustodian != null && filter.AssigneeCustodian != 0)
                predicate = predicate.And(x => x.ASSIGNEE_CUSTODIAN == filter.AssigneeCustodian);
            if (!string.IsNullOrEmpty(filter.RequestedBy)) predicate = (strict)
                     ? predicate.And(x => x.REQUESTED_BY.ToLower() == filter.RequestedBy.ToLower())
                     : predicate.And(x => x.REQUESTED_BY.Contains(filter.RequestedBy.ToLower()));
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                     ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                     : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                     ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                     : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
            if (!string.IsNullOrEmpty(filter.ResponsibilityCode)) predicate = (strict)
                     ? predicate.And(x => x.RESPONSIBILITY_CODE.ToLower() == filter.ResponsibilityCode.ToLower())
                     : predicate.And(x => x.RESPONSIBILITY_CODE.Contains(filter.ResponsibilityCode.ToLower()));
            if (filter.UnitCost != null && filter.UnitCost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.UnitCost);
            if (filter.Qty != null && filter.Qty != 0)
                predicate = predicate.And(x => x.QTY == filter.Qty);
            if (filter.SalvageValue != null && filter.SalvageValue != 0)
                predicate = predicate.And(x => x.SALVAGE_VALUE == filter.SalvageValue);
            if (filter.BookValue != null && filter.BookValue != 0)
                predicate = predicate.And(x => x.BOOK_VALUE == filter.BookValue);
            if (filter.EstLife != null && filter.EstLife != 0)
                predicate = predicate.And(x => x.ESTIMATED_LIFE == filter.EstLife);
            if (filter.Area != null && filter.Area != 0)
                predicate = predicate.And(x => x.AREA == filter.Area);
            if (filter.Semi != null && filter.Semi != 0)
                predicate = predicate.And(x => x.SEMI_EXPANDABLE_AMOUNT == filter.Semi);
            if (!string.IsNullOrEmpty(filter.UserStamp)) predicate = (strict)
                    ? predicate.And(x => x.USER_STAMP.ToLower() == filter.UserStamp.ToLower())
                    : predicate.And(x => x.USER_STAMP.Contains(filter.UserStamp.ToLower()));
            if (filter.TimeStamp != null && filter.TimeStamp != DateTime.MinValue)
                predicate = predicate.And(x => x.TIME_STAMP == filter.TimeStamp);
            if (!string.IsNullOrEmpty(filter.Invoice)) predicate = (strict)
                    ? predicate.And(x => x.INVOICE.ToLower() == filter.Invoice.ToLower())
                    : predicate.And(x => x.INVOICE.Contains(filter.Invoice.ToLower()));
            if (!string.IsNullOrEmpty(filter.PropertyCondition)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_CONDITION.ToLower() == filter.PropertyCondition.ToLower())
                     : predicate.And(x => x.PROPERTY_CONDITION.Contains(filter.PropertyCondition.ToLower()));

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDetailsDTO> Update(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);

            _ctx.Entry(data).State = data.ID == 0 ? EntityState.Added : EntityState.Modified; ;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<EamisPropertyTransactionDetailsDTO> getPropertyItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisPropertyTransactionDetailsDTO
            {
                Id = result.ID,
                PropertyTransactionID = result.PROPERTY_TRANS_ID,
                isDepreciation = result.IS_DEPRECIATION,
                Dr = result.DR,
                ItemCode = result.ITEM_CODE,
                PropertyNumber = result.PROPERTY_NUMBER,
                ItemDescription = result.ITEM_DESCRIPTION,
                SerialNumber = result.SERIAL_NUMBER,
                Po = result.PO,
                Pr = result.PR,
                AcquisitionDate = result.ACQUISITION_DATE,
                AssigneeCustodian = result.ASSIGNEE_CUSTODIAN,
                RequestedBy = result.REQUESTED_BY,
                Office = result.OFFICE,
                Department = result.DEPARTMENT,
                ResponsibilityCode = result.RESPONSIBILITY_CODE,
                UnitCost = result.UNIT_COST,
                Qty = result.QTY,
                SalvageValue = result.SALVAGE_VALUE,
                BookValue = result.BOOK_VALUE,
                EstLife = result.ESTIMATED_LIFE,
                Area = result.AREA,
                Semi = result.SEMI_EXPANDABLE_AMOUNT,
                UserStamp = result.USER_STAMP,
                TimeStamp = result.TIME_STAMP,
                WarrantyExpiry = result.WARRANTY_EXPIRY,
                Invoice = result.INVOICE,
                PropertyCondition = result.PROPERTY_CONDITION
                ,
                PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(x => new EamisPropertyTransactionDTO
                {
                    Id = x.ID,
                    DeliveryDate = x.DELIVERY_DATE,
                    TransactionStatus = x.TRANSACTION_STATUS,
                    TransactionNumber = x.TRANSACTION_NUMBER,
                    TransactionDate = x.TRANSACTION_DATE,
                    FiscalPeriod = x.FISCALPERIOD,
                    ReceivedBy = x.RECEIVED_BY
                }).Where(i => i.Id == result.PROPERTY_TRANS_ID).FirstOrDefault()
            };
        }

        public async Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item)
        {
            string strResult = "";
            //check item in DB
            var itemInDB = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.FirstOrDefault(i => i.ID == item.ItemId)).ConfigureAwait(false);
            if (itemInDB != null)
            {
                itemInDB.QUANTITY = itemInDB.QUANTITY + item.QtyReceived;
                var result = await _ctx.SaveChangesAsync();
                if (result > 0)
                {
                    strResult = "Successfully updated.";
                }
            }
            return strResult;
        }
        partial class IssuedQtyDTO
        {
            public string ItemCode { get; set; }
            public string PO { get; set; }
            public int IssuedQty { get; set; }
        }
    }

}
