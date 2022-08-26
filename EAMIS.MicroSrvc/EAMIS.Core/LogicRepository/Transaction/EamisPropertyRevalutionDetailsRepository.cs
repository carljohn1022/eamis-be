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
    public class EamisPropertyRevalutionDetailsRepository : IEamisPropertyRevalutionDetailsRepository
    {

        private readonly EAMISContext _ctx;
        private readonly IFactorType _factorType;
        private readonly int _maxPageSize;

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
        public EamisPropertyRevalutionDetailsRepository(EAMISContext ctx, IFactorType factorType)
        {
            _ctx = ctx;
            _factorType = factorType;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisPropertyRevaluationDetailsDTO> Insert(EamisPropertyRevaluationDetailsDTO item)
        {
            EAMISPROPERTYREVALUATIONDETAILS data = MapToEntity(item);
            try
            {
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
            return item;
        }

        private void GetPropertyItemDetails(string itemCode)
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
            }
        }

        public EamisPropertyRevaluationDetailsDTO CalculateRevaluationDetails(EamisPropertyRevaluationDetailsDTO item, DateTime? newDepreciationDate)
        {

            newDepreciationDate =  newDepreciationDate == null ? DateTime.Now : newDepreciationDate;
            int runningLife = ((newDepreciationDate.Value.Year - item.Depreciation.Year) * 12) + (newDepreciationDate.Value.Month - item.Depreciation.Month);

            if (runningLife > 0) // if at least a month then calculate depreciation based on item's age
            {
                decimal salvageValue = _factorType.GetFactorTypeValue(FactorTypes.SalvageValue); //Get salvage value factor
                decimal bookValue = item.AcquisitionCost - (item.AcquisitionCost * salvageValue); //Unit Cost * Salvage value factor
                decimal monthlyDepreciation = bookValue / EstimatedLife; //Estimated life source is Item_Category

                item.AccumulativeDepreciation = monthlyDepreciation * runningLife; //Monthly Depreciation, to confirmed
                item.NetBookValue = bookValue - (monthlyDepreciation * runningLife);
                item.SalvageValue = item.AcquisitionCost * salvageValue;
                item.RemainingLife = item.RemainingLife - runningLife;
                item.RevaluedAmount = 0; //to do: update once value to be assigned is final/confirmed
                item.FairValue = bookValue;
                item.PrevRevaluation = string.Empty; //to do: update once value to be assigned is final/confirmed.
                item.NewDep = newDepreciationDate.Value; //to do: update once value to be assigned is final/confirmed
            }
            return item;
        }

        private EAMISPROPERTYREVALUATIONDETAILS MapToEntity(EamisPropertyRevaluationDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYREVALUATIONDETAILS();
            return new EAMISPROPERTYREVALUATIONDETAILS
            {
                ID = item.Id,
                ACCUMULATIVE_DEPRECIATION = item.AccumulativeDepreciation,
                ACQ_COST = item.AcquisitionCost,
                DEPRECIATION = item.Depreciation,
                FAIR_VALUE = item.FairValue,
                ITEM_CODE = item.ItemCode,
                ITEM_DESC = item.ItemDescription,
                NET_BOOK_VALUE = item.NetBookValue,
                NEW_DEP = item.NewDep,
                PREV_REVALUATION = item.PrevRevaluation,
                PROPERTY_REVALUATION_ID = item.PropertyRevaluationId, //PropertyRevaluation.Id
                REMAINING_LIFE = item.RemainingLife,
                REVALUED_AMT = item.RevaluedAmount,
                SALVAGE_VALUE = item.SalvageValue
            };
        }


        public async Task<DataList<EamisPropertyRevaluationDetailsDTO>> List(EamisPropertyRevaluationDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYREVALUATIONDETAILS> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyRevaluationDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISPROPERTYREVALUATIONDETAILS> FilteredEntites(EamisPropertyRevaluationDetailsDTO filter, IQueryable<EAMISPROPERTYREVALUATIONDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYREVALUATIONDETAILS>(true);

            var arrDistinctDetailID = _ctx.EAMIS_PROPERTY_REVALUATION_DETAILS
                                          .GroupBy(x => x.PROPERTY_REVALUATION_ID)
                                          .Select(i => i.Max(x => x.ID))
                                          .ToList();
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_REVALUATION_DETAILS.Where(x => arrDistinctDetailID.Contains(x.ID));
                                        
            return query.Where(predicate);
        }
        private IQueryable<EamisPropertyRevaluationDetailsDTO> QueryToDTO(IQueryable<EAMISPROPERTYREVALUATIONDETAILS> query)
        {
            return query.Select(d => new EamisPropertyRevaluationDetailsDTO
            {
                Id = d.ID,
                AccumulativeDepreciation = d.ACCUMULATIVE_DEPRECIATION,
                AcquisitionCost = d.ACQ_COST,
                Depreciation = d.DEPRECIATION,
                FairValue = d.FAIR_VALUE,
                ItemCode = d.ITEM_CODE,
                ItemDescription = d.ITEM_DESC,
                NetBookValue = d.NET_BOOK_VALUE,
                NewDep = d.NEW_DEP,
                PrevRevaluation = d.PREV_REVALUATION,
                RemainingLife = d.REMAINING_LIFE,
                RevaluedAmount = d.REVALUED_AMT,
                PropertyRevaluationId = d.PROPERTY_REVALUATION_ID,
                PropertyRevaluation = _ctx.EAMIS_PROPERTY_REVALUATION.AsNoTracking().Select(x => new EamisPropertyRevaluationDTO
                {
                    Id = x.ID,
                    TransactionId = x.TRAN_ID,
                    TransactionDate = x.TRAN_DATE,
                    Particulars = x.PARTICULARS,
                    IsActive = x.IS_ACTIVE,
                }
                ).Where(dt => dt.Id == d.PROPERTY_REVALUATION_ID).FirstOrDefault()
            });
        }

        private IQueryable<EAMISPROPERTYREVALUATIONDETAILS> PagedQuery(IQueryable<EAMISPROPERTYREVALUATIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisPropertyRevaluationDetailsDTO> Update(EamisPropertyRevaluationDetailsDTO item)
        {
            if (item.Id == 0)
            {
                _errorMessage = "Record does not exist.";
                bolerror = true;
                return null;
            }
            EAMISPROPERTYREVALUATIONDETAILS data = MapToEntity(item);
            try
            {
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
            return item;
        }

        #region Creation of Property Item for Revaluation
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
                Appraisalincrement = d.APPRAISAL_INCREMENT,
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


        #endregion Creation of Property Item for Revaluation
    }
}
