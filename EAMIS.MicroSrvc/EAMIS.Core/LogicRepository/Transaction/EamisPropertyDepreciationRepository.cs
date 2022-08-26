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
        private readonly int _maxPageSize;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }

        private int _estimatedLife;
        private int EstimatedLife { get => _estimatedLife; set => value = _estimatedLife; }

        private List<TempScheduleDTO> lsttempScheduleDTO = new List<TempScheduleDTO>();
        public EamisPropertyDepreciationRepository(EAMISContext ctx, IFactorType factorType)
        {
            _ctx = ctx;
            _factorType = factorType;
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
                    WarrantyDate = d.WARRANTY_DATE
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
                result.Items[intResult].DepreciationAmount = lsttempScheduleDTO.Find(i => i.ID == result.Items[intResult].PropertyScheduleId).DepreciationAmount;

            return result;
            //return new DataList<EamisPropertyDepreciationDTO>
            //{
            //    Count = await query.CountAsync(),
            //    Items = await QueryToDTOForDepreciationCreation(paged).ToListAsync(),
            //};
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
                                    .Select(x => new
                                    {
                                        x.ID,
                                        x.ACQUISITION_DATE,
                                        x.ACQUISITION_COST,
                                        x.ITEM_CODE
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
                if ((runningLife > 0) && (EstimatedLife > runningLife))
                {
                    //Determine/forecast next depreciation month and year if within the given year and month
                    DateTime nextDepreciationDate = item.ACQUISITION_DATE.AddMonths(runningLife);

                    //Check next depreciation month and year is matched with the given year and month
                    if (filter.DepreciationYear == nextDepreciationDate.Year &&
                       filter.DepreciationMonth == nextDepreciationDate.Month)
                    {
                        arrItemsForDepreciation.Add(item.ID);

                        //calculate depreciation

                        decimal salvageValue = _factorType.GetFactorTypeValue(FactorTypes.SalvageValue); //Get salvage value factor
                        decimal bookValue = item.ACQUISITION_COST - (item.ACQUISITION_COST * salvageValue); //Acquisition Cost * Salvage value factor
                        decimal monthlyDepreciation = bookValue / EstimatedLife; //Estimated life source is Item_Category

                        TempScheduleDTO tempScheduleDTO = new TempScheduleDTO();
                        tempScheduleDTO.ID = item.ID;
                        //tempScheduleDTO.DEPRECIATION_AMOUNT = monthlyDepreciation * runningLife;
                        tempScheduleDTO.AcquisitionDate = item.ACQUISITION_DATE;
                        tempScheduleDTO.AcquisitionCost = item.ACQUISITION_COST;
                        tempScheduleDTO.EstimatedLife = EstimatedLife; //get from category
                        tempScheduleDTO.RunningLife = runningLife;
                        tempScheduleDTO.DepreciationAmount = Convert.ToDecimal((monthlyDepreciation * runningLife).ToString("#0.00"));
                        lsttempScheduleDTO.Add(tempScheduleDTO);

                    }
                }
            }

            //get the final list from database based on the above filter
            //var query = custom_query ?? _ctx.EAMIS_PROPERTY_DEPRECIATION.AddRange((IEnumerable<EAMISPROPERTYDEPRECIATION>)lsttempScheduleDTO);
            //foreach (var item in lsttempScheduleDTO)
            //{
            //    _ctx.EAMIS_PROPERTY_DEPRECIATION (item);
            //}
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
            //.Select(x => new EAMISPROPERTYDEPRECIATION
            //{
            //    ID = 0,
            //    DEPRECIATION_AMOUNT = lsttempScheduleDTO.,
            //    PROPERTY_SCHEDULE_ID = lsttempScheduleDTO.ID, //Property Schedule Id
            //    DEPRECIATION_MONTH = filter.DepreciationMonth,
            //    DEPRECIATION_YEAR = filter.DepreciationYear,
            //    POSTING_DATE = DateTime.Now
            //});
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
                    WarrantyDate = d.WARRANTY_DATE
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
        public decimal DepreciationAmount { get; set; }
        public int PropertyScheduleId { get; set; }
    }
}
