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
    public class EamisPropertyDepreciationRepository : IEamisPropertyDepreciationRepository
    {
        private readonly EAMISContext _ctx;
        private readonly IFactorType _factorType;
        private readonly IEamisPropertyScheduleRepository _eamisPropertyScheduleRepository;
        private readonly int _maxPageSize;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }

        private int _estimatedLife;
        private int EstimatedLife { get => _estimatedLife; set => value = _estimatedLife; }

        private List<TempScheduleDTO> lsttempScheduleDTO = new List<TempScheduleDTO>();
        public EamisPropertyDepreciationRepository(EAMISContext ctx, IFactorType factorType,
            IEamisPropertyScheduleRepository eamisPropertyScheduleRepository)
        {
            _ctx = ctx;
            _factorType = factorType;
            _eamisPropertyScheduleRepository = eamisPropertyScheduleRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisPropertyDepreciationDTO> Delete(EamisPropertyDepreciationDTO item)
        {
            var itemInDB = _ctx.EAMIS_PROPERTY_DEPRECIATION.Where(x => x.PROPERTY_SCHEDULE_ID == item.PropertyScheduleId &&
                                                                       x.DEPRECIATION_YEAR == item.DepreciationYear &&
                                                                       x.DEPRECIATION_MONTH == item.DepreciationMonth).FirstOrDefault();

            if (itemInDB == null)
            {
                _errorMessage = "Record does not exist.";
                bolerror = true;
                return new EamisPropertyDepreciationDTO();
            }

            EAMISPROPERTYDEPRECIATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisPropertyDepreciationDTO> Insert(EamisPropertyDepreciationDTO item)
        {
            var itemInDB = _ctx.EAMIS_PROPERTY_DEPRECIATION.Where(x => x.PROPERTY_SCHEDULE_ID == item.PropertyScheduleId &&
                                                                       x.DEPRECIATION_YEAR == item.DepreciationYear &&
                                                                       x.DEPRECIATION_MONTH == item.DepreciationMonth).FirstOrDefault();

            if (itemInDB != null)
            {
                _errorMessage = "Record already exist.";
                bolerror = true;
                return new EamisPropertyDepreciationDTO();
            }

            EAMISPROPERTYDEPRECIATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            item.Id = data.ID;

            //update property schedule
            EamisPropertyScheduleDTO schedule = new EamisPropertyScheduleDTO
            {
                Id = item.PropertyScheduleId,
                AccumulatedDepreciationAmount = item.DepreciationAmount,
                RemainingLife = item.PropertyScheduleDetails.RemainingLife,
                AcquisitionCost = item.PropertyScheduleDetails.AcquisitionCost,
                BookValue = item.PropertyScheduleDetails.BookValue,

                AcquisitionDate = item.PropertyScheduleDetails.AcquisitionDate,
                Appraisalincrement = item.PropertyScheduleDetails.Appraisalincrement,
                AppraisedValue = item.PropertyScheduleDetails.AppraisedValue,
                AreaSQM = item.PropertyScheduleDetails.AreaSQM,
                AssessedValue = item.PropertyScheduleDetails.AssessedValue,
                AssetCondition = item.PropertyScheduleDetails.AssetCondition,
                AssetTag = item.PropertyScheduleDetails.AssetTag,
                Category = item.PropertyScheduleDetails.Category,
                CostCenter = item.PropertyScheduleDetails.CostCenter,
                Department = item.PropertyScheduleDetails.Department,
                DeprecAmount = item.PropertyScheduleDetails.DeprecAmount,
                Details = item.PropertyScheduleDetails.Details,
                DisposedAmount = item.PropertyScheduleDetails.DisposedAmount,
                ESTLife = item.PropertyScheduleDetails.ESTLife,
                ForDepreciation = item.PropertyScheduleDetails.ForDepreciation,
                InvoiceNo = item.PropertyScheduleDetails.InvoiceNo,
                ItemDescription = item.PropertyScheduleDetails.ItemDescription,
                LastDepartment = item.PropertyScheduleDetails.LastDepartment,
                LastPostedDate = item.PropertyScheduleDetails.LastPostedDate,
                Location = item.PropertyScheduleDetails.Location,
                Names = item.PropertyScheduleDetails.Names,
                PORef = item.PropertyScheduleDetails.PORef,
                PropertyNumber = item.PropertyScheduleDetails.PropertyNumber,
                RealEstateTaxPayment = item.PropertyScheduleDetails.RealEstateTaxPayment,
                RevaluationCost = item.PropertyScheduleDetails.RevaluationCost,
                RRDate = item.PropertyScheduleDetails.RRDate,
                RRRef = item.PropertyScheduleDetails.RRRef,
                SalvageValue = item.PropertyScheduleDetails.SalvageValue,
                SerialNo = item.PropertyScheduleDetails.SerialNo,
                Status = item.PropertyScheduleDetails.Status,
                SubCategory = item.PropertyScheduleDetails.SubCategory,
                SvcAgreementNo = item.PropertyScheduleDetails.SvcAgreementNo,
                VendorName = item.PropertyScheduleDetails.VendorName,
                Warranty = item.PropertyScheduleDetails.Warranty,
                WarrantyDate = item.PropertyScheduleDetails.WarrantyDate,
                ReferenceId = item.PropertyScheduleDetails.ReferenceId,
                ItemCode = item.PropertyScheduleDetails.ItemCode
            };
            var result = await _eamisPropertyScheduleRepository.Update(schedule);
            return item;
        }

        public async Task<EamisPropertyDepreciationDTO> Update(EamisPropertyDepreciationDTO item)
        {
            var itemInDB = _ctx.EAMIS_PROPERTY_DEPRECIATION.Where(x => x.PROPERTY_SCHEDULE_ID == item.PropertyScheduleId &&
                                                                       x.DEPRECIATION_YEAR == item.DepreciationYear &&
                                                                       x.DEPRECIATION_MONTH == item.DepreciationMonth).FirstOrDefault();

            if (itemInDB == null)
            {
                _errorMessage = "Record does not exist.";
                bolerror = true;
                return new EamisPropertyDepreciationDTO();
            }
            EAMISPROPERTYDEPRECIATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPROPERTYDEPRECIATION MapToEntity(EamisPropertyDepreciationDTO item)
        {
            if (item == null) return new EAMISPROPERTYDEPRECIATION();
            return new EAMISPROPERTYDEPRECIATION
            {
                ID = item.Id,
                DEPRECIATION_AMOUNT = item.DepreciationAmount,
                DEPRECIATION_MONTH = item.DepreciationMonth,
                DEPRECIATION_YEAR = item.DepreciationYear,
                POSTING_DATE = item.PostingDate,
                PROPERTY_SCHEDULE_ID = item.PropertyScheduleId
            };
        }


        public async Task<DataList<EamisPropertyDepreciationDTO>> List(EamisPropertyDepreciationDTO filter, PageConfig config)
        {

            IQueryable<EAMISPROPERTYDEPRECIATION> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_IsAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyDepreciationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EAMISPROPERTYDEPRECIATION> FilteredEntities(EamisPropertyDepreciationDTO filter, IQueryable<EAMISPROPERTYDEPRECIATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYDEPRECIATION>(true);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_DEPRECIATION;
            return query.Where(predicate);
        }

        private IQueryable<EamisPropertyDepreciationDTO> QueryToDTO(IQueryable<EAMISPROPERTYDEPRECIATION> query)
        {
            return query.Select(x => new EamisPropertyDepreciationDTO
            {
                Id = x.ID,
                PropertyScheduleId = x.PROPERTY_SCHEDULE_ID,
                DepreciationAmount = x.DEPRECIATION_AMOUNT,
                DepreciationMonth = x.DEPRECIATION_MONTH,
                DepreciationYear = x.DEPRECIATION_YEAR,
                PostingDate = x.POSTING_DATE,
                PropertyScheduleDetails = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Select(d => new EamisPropertyScheduleDTO
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
                    WarrantyDate = d.WARRANTY_DATE,
                    ReferenceId = d.REFERENCE_ID
                }).Where(i => i.Id == x.PROPERTY_SCHEDULE_ID).FirstOrDefault()
            });
        }

        private IQueryable<EAMISPROPERTYDEPRECIATION> PagedQuery(IQueryable<EAMISPROPERTYDEPRECIATION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        #region property schedule for depreciation listing
        public async Task<DataList<EamisPropertyDepreciationDTO>> ListForDepreciationCreation(EamisPropertyDepreciationDTO filter, PageConfig config)
        {
            //-List all items from property schedule, based on the given parameters
            //- Year, Month(basis is Acquisition Date)
            //Month and year must be specified in the EamisPropertyDepreciationDTO
            IQueryable<EAMISPROPERTYDEPRECIATION> query = FilteredEntitiesForDepreciationCreation(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_IsAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);

            var result =
             new DataList<EamisPropertyDepreciationDTO>
             {
                 Count = await query.CountAsync(),
                 Items = await QueryToDTO(paged).ToListAsync(),
             };

            for (int intResult = 0; intResult < result.Items.Count; intResult++)
            {
                result.Items[intResult].DepreciationAmount = lsttempScheduleDTO.Find(i => i.ID == result.Items[intResult].PropertyScheduleId).DepreciationAmount;
                result.Items[intResult].PropertyScheduleDetails.RemainingLife = lsttempScheduleDTO.Find(i => i.ID == result.Items[intResult].PropertyScheduleId).RemainingLife;
                result.Items[intResult].PropertyScheduleDetails.BookValue = lsttempScheduleDTO.Find(i => i.ID == result.Items[intResult].PropertyScheduleId).NewBookValue;
            }
            return result;
        }

        private void GetEstimatedLife(string itemCode)
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
                            c.itemSubCategory.category.ESTIMATED_LIFE
                        }).FirstOrDefault();
            if (result != null)
                _estimatedLife = result.ESTIMATED_LIFE;
        }

        private bool IsDepreciationDue(int propertyScheduleId)
        {


            return false;
        }

        private IQueryable<EAMISPROPERTYDEPRECIATION> FilteredEntitiesForDepreciationCreation(EamisPropertyDepreciationDTO filter, IQueryable<EAMISPROPERTYDEPRECIATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYDEPRECIATION>(true);

            //Compute running estimated life based on acquisition date

            var propertySchedule = _ctx.EAMIS_PROPERTY_SCHEDULE
                                    //.Where(i => i.ID == 46)
                                    .Select(x => new
                                    {
                                        x.ID,
                                        x.ACQUISITION_DATE,
                                        x.ACQUISITION_COST,
                                        x.ITEM_CODE,
                                        x.SALVAGE_VALUE
                                    }
                                    ).ToList();
            string newDepreciationDate = filter.DepreciationMonth.ToString() + "/1/" + filter.DepreciationYear; //set to first day of the month. 



            //Determine items that are due for depreciation based on the given month and year
            var arrItemsForDepreciation = new List<int>();
            //List<EAMISPROPERTYDEPRECIATION> lsttempScheduleDTO = new List<EAMISPROPERTYDEPRECIATION>();
            foreach (var item in propertySchedule)
            {
                //compute running life


                int runningLife = ((filter.DepreciationYear - item.ACQUISITION_DATE.Year) * 12) + (filter.DepreciationMonth - item.ACQUISITION_DATE.Month);
                GetEstimatedLife(item.ITEM_CODE);
                DateTime nextDepreciationDate;
                if (runningLife == 0)
                {
                    //1-15 current month -> subject for depreciation for the month -> 1st depreciation
                    nextDepreciationDate = item.ACQUISITION_DATE;
                    runningLife = 1;
                }
                else
                {
                    //Determine/forecast next depreciation month and year if within the given year and month
                    //16-end of the month -> due for next month -> 1st depreciation
                    //succeeding depreciation -> 
                    nextDepreciationDate = item.ACQUISITION_DATE.AddMonths(runningLife);
                    if (item.ACQUISITION_DATE.Day < PropertyItemStatus.CutOffDay)
                        runningLife += 1; //always add 1 to running life when items acquired day  between 1 and 15
                }

                if ((runningLife >= 0) && (EstimatedLife > runningLife))
                {
                    //Check next depreciation month and year is matched with the given year and month
                    if (filter.DepreciationYear == nextDepreciationDate.Year &&
                       filter.DepreciationMonth == nextDepreciationDate.Month)
                    {
                        arrItemsForDepreciation.Add(item.ID);

                        //calculate depreciation
                        //salvage value = item.ACQUISITION_COST * salvageValue
                        //decimal salvageValue = _factorType.GetFactorTypeValue(FactorTypes.SalvageValue); //Get salvage value factor
                        decimal bookValue = item.ACQUISITION_COST - item.SALVAGE_VALUE; //Acquisition Cost - Salvage Value
                        decimal monthlyDepreciation = bookValue / EstimatedLife; //Estimated life source is Item_Category

                        TempScheduleDTO tempScheduleDTO = new TempScheduleDTO();
                        decimal newDepreciationAmount = Convert.ToDecimal(monthlyDepreciation.ToString("#0.00")) * runningLife;
                        tempScheduleDTO.ID = item.ID;
                        //tempScheduleDTO.DEPRECIATION_AMOUNT = monthlyDepreciation * runningLife;
                        tempScheduleDTO.AcquisitionDate = item.ACQUISITION_DATE;
                        tempScheduleDTO.AcquisitionCost = item.ACQUISITION_COST;
                        tempScheduleDTO.EstimatedLife = EstimatedLife; //get from category
                        tempScheduleDTO.RunningLife = runningLife;
                        tempScheduleDTO.DepreciationAmount = newDepreciationAmount;
                        tempScheduleDTO.SalvageValue = item.SALVAGE_VALUE;
                        tempScheduleDTO.RemainingLife = EstimatedLife - runningLife;
                        tempScheduleDTO.NewBookValue = item.ACQUISITION_COST - newDepreciationAmount;
                        lsttempScheduleDTO.Add(tempScheduleDTO);
                    }
                }
            }

            //get the final list from database based on the above filter
            
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_SCHEDULE.Where(x => arrItemsForDepreciation.Contains(x.ID))
                                            .Select(d => new EAMISPROPERTYDEPRECIATION
                                            {
                                                ID = 0,
                                                PROPERTY_SCHEDULE_ID = d.ID,
                                                DEPRECIATION_AMOUNT = 0,
                                                DEPRECIATION_MONTH = filter.DepreciationMonth,
                                                DEPRECIATION_YEAR = filter.DepreciationYear,
                                                POSTING_DATE = DateTime.Now
                                            });
            
            return query.Where(predicate);
        }

        private IQueryable<EamisPropertyDepreciationDTO> QueryToDTOForDepreciationCreation(IQueryable<EAMISPROPERTYDEPRECIATION> query)
        {
            var result = query.Select(x => new EamisPropertyDepreciationDTO
            {
                Id = x.ID,
                PropertyScheduleId = x.PROPERTY_SCHEDULE_ID,
                DepreciationAmount = x.DEPRECIATION_AMOUNT,
                DepreciationMonth = x.DEPRECIATION_MONTH,
                DepreciationYear = x.DEPRECIATION_YEAR,
                PostingDate = x.POSTING_DATE,
                PropertyScheduleDetails = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Select(d => new EamisPropertyScheduleDTO
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
                    WarrantyDate = d.WARRANTY_DATE,
                    ReferenceId = d.REFERENCE_ID,
                    RemainingLife = d.REMAINING_LIFE
                }).Where(i => i.Id == x.PROPERTY_SCHEDULE_ID).FirstOrDefault()
            });

            return result;
        }

        #endregion property schedule for depreciation listing
    }

    partial class TempScheduleDTO
    {
        public int ID { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal AcquisitionCost { get; set; }
        public int EstimatedLife { get; set; }
        public int RunningLife { get; set; }
        public decimal SalvageValue { get; set; }
        public decimal DepreciationAmount { get; set; }
        public int PropertyScheduleId { get; set; }
        public int RemainingLife { get; set; }
        public decimal NewBookValue { get; set; }
    }
}
