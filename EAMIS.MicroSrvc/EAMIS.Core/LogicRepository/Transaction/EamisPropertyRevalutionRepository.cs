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
    public class EamisPropertyRevalutionRepository : IEamisPropertyRevalutionRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyRevalutionRepository(EAMISContext ctx,
                                                 IEAMISIDProvider EAMISIDProvider)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<string> GetNextSequenceNumber()
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumber(TransactionTypeSettings.PropertyRevaluation);
            return nextId;
        }

        public async Task<EamisPropertyRevaluationDTO> Update(EamisPropertyRevaluationDTO item)
        {
            if(item.Id == 0)
            {
                _errorMessage = "Record does not exist.";
                bolerror = true;
                return null;
            }    
            EAMISPROPERTYREVALUATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisPropertyRevaluationDTO> Insert(EamisPropertyRevaluationDTO item)
        {
            try
            {
                EAMISPROPERTYREVALUATION data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                //ensure that recently added record has correct transaction id number
                item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

                string _drType = PrefixSettings.PVPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

                if (item.TransactionId != _drType)
                {
                    item.TransactionId = _drType;

                    //reset context state to avoid error
                    _ctx.Entry(data).State = EntityState.Detached;

                    //call the update method, force to update the transaction number in DB
                    await this.Update(item);
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
            return item;
        }

        private EAMISPROPERTYREVALUATION MapToEntity(EamisPropertyRevaluationDTO item)
        {
            if (item == null) return new EAMISPROPERTYREVALUATION();
            return new EAMISPROPERTYREVALUATION
            {
                ID = item.Id,
                TRAN_DATE = item.TransactionDate,
                TRAN_ID = item.TransactionId,
                PARTICULARS = item.Particulars,
                USER_STAMP = item.UserStamp,
                STATUS = item.Status
            };
        }
        
        public async Task<DataList<EamisPropertyRevaluationDTO>> List(EamisPropertyRevaluationDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYREVALUATION> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyRevaluationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISPROPERTYREVALUATION> FilteredEntites(EamisPropertyRevaluationDTO filter, IQueryable<EAMISPROPERTYREVALUATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYREVALUATION>(true);
            
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_REVALUATION;
            return query.Where(predicate);
        }
        private IQueryable<EamisPropertyRevaluationDTO> QueryToDTO(IQueryable<EAMISPROPERTYREVALUATION> query)
        {
            return query.Select(x => new EamisPropertyRevaluationDTO
            {
                Id = x.ID,
                TransactionId = x.TRAN_ID,
                TransactionDate = x.TRAN_DATE,
                Particulars = x.PARTICULARS,
                IsActive = x.IS_ACTIVE,
                Status = x.STATUS,
                PropertyRevaluationDetails = _ctx.EAMIS_PROPERTY_REVALUATION_DETAILS.AsNoTracking().Select(d => new EamisPropertyRevaluationDetailsDTO 
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
                    PropertyRevaluationId = d.PROPERTY_REVALUATION_ID
                }
                ).Where(dt => dt.PropertyRevaluationId == x.ID).ToList()
            });
        }

        private IQueryable<EAMISPROPERTYREVALUATION> PagedQuery(IQueryable<EAMISPROPERTYREVALUATION> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EAMISPROPERTYREVALUATION> PagedQueryForSearch(IQueryable<EAMISPROPERTYREVALUATION> query)
        {
            return query;
        }
        public async Task<DataList<EamisPropertyRevaluationDTO>> SearchPropertyRevaluation(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYREVALUATION> query = null;
            if (type == "Transaction Id")
            {
                query = _ctx.EAMIS_PROPERTY_REVALUATION.AsNoTracking().Where(x => x.TRAN_ID.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Transaction Date")
            {
                query = _ctx.EAMIS_PROPERTY_REVALUATION.AsNoTracking().Where(x => x.TRAN_DATE.ToString().Contains(searchValue)).AsQueryable();
            }
            else if (type == "Particulars")
            {
                query = _ctx.EAMIS_PROPERTY_REVALUATION.AsNoTracking().Where(x => x.PARTICULARS.Contains(searchValue)).AsQueryable();
            }
            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyRevaluationDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        public async Task<EamisPropertyRevaluationDTO> getAssetItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_REVALUATION.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisPropertyRevaluationDTO
            {
                Id = result.ID,
                TransactionId = result.TRAN_ID,
                TransactionDate = result.TRAN_DATE,
                Particulars = result.PARTICULARS,
                IsActive = result.IS_ACTIVE,
                Status = result.STATUS,
                PropertyRevaluationDetails = _ctx.EAMIS_PROPERTY_REVALUATION_DETAILS.AsNoTracking().Select(x => new EamisPropertyRevaluationDetailsDTO
                {
                    Id = x.ID,
                    PropertyRevaluationId = x.PROPERTY_REVALUATION_ID,
                    ItemCode = x.ITEM_CODE,
                    ItemDescription = x.ITEM_DESC,
                    AcquisitionCost = x.ACQ_COST,
                    Depreciation = x.DEPRECIATION,
                    RemainingLife = x.REMAINING_LIFE,
                    AccumulativeDepreciation = x.ACCUMULATIVE_DEPRECIATION,
                    PrevRevaluation = x.PREV_REVALUATION,
                    NetBookValue = x.NET_BOOK_VALUE,
                    RevaluedAmount = x.REVALUED_AMT,
                    FairValue = x.FAIR_VALUE,
                    SalvageValue = x.SALVAGE_VALUE,
                    DepPerMonth = x.DEP_PER_MONTH,
                    NewDepPerMonth = x.NEW_DEP_PER_MONTH
                }).Where(i => i.PropertyRevaluationId == result.ID).ToList()
            };
        }

    }
}
