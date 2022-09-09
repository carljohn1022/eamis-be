using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
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

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisFundSourceRepository : IEamisFundSourceRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisFundSourceRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                           : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<bool> InsertFromExcel(List<EamisFundSourceDTO> Items)
        {
            List<EAMISFUNDSOURCE> lstFund = new List<EAMISFUNDSOURCE>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISFUNDSOURCE objFund = MapToEntity(Items[intItems]);

                    lstFund.Add(objFund);
                }
                _ctx.EAMIS_FUND_SOURCE.AddRange(lstFund);
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
                bolerror = false;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.InnerException.Message;
            }
            return HasError;
        }
        public async Task<EamisFundSourceDTO> InsertFromExcel(EamisFundSourceDTO item)
        {
            try
            {
                EAMISFUNDSOURCE data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return item;
        }
        public async Task<DataList<EamisFundSourceDTO>> List(EamisFundSourceDTO filter, PageConfig config)
        {
            IQueryable<EAMISFUNDSOURCE> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisFundSourceDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).OrderByDescending(x => x.Id).ToListAsync(),
            };
        }

        public async Task<DataList<EamisFundSourceDTO>> SearchFunds(string type, string searchValue)
        {
            IQueryable<EAMISFUNDSOURCE> query = null;
            if(type == "Fund")
            {
                query = _ctx.EAMIS_FUND_SOURCE.AsNoTracking().Where(x => x.GENERALFUNDSOURCE.NAME.Contains(searchValue)).AsQueryable();
            }
            else if(type == "Code")
            {
                query = _ctx.EAMIS_FUND_SOURCE.AsNoTracking().Where(x => x.CODE.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_FUND_SOURCE.AsNoTracking().Where(x => x.FUND_CATEGORY.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisFundSourceDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        public async Task<DataList<EamisFundSourceDTO>> searchFundForIssuance (string searchValue)
        {
            IQueryable<EAMISFUNDSOURCE> query = null;
            query = _ctx.EAMIS_FUND_SOURCE.AsNoTracking().Where(x => x.FUND_CATEGORY.Contains(searchValue)).AsQueryable();

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisFundSourceDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISFUNDSOURCE> PagedQueryForSearch(IQueryable<EAMISFUNDSOURCE> query)
        {
            return query;
        }

        private IQueryable<EAMISFUNDSOURCE> PagedQuery(IQueryable<EAMISFUNDSOURCE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }


        private IQueryable<EamisFundSourceDTO> QueryToDTO(IQueryable<EAMISFUNDSOURCE> query)
        {
            return query.Select(x => new EamisFundSourceDTO
            {
                Id = x.ID,
                GenFundId = x.GENERAL_FUND_SOURCE_ID,
                Code = x.CODE,
                AuthorizationId = x.AUTHORIZATION_ID,
                FinancingSourceId = x.FINANCING_SOURCE_ID,
                FundCategory = x.FUND_CATEGORY,
                IsActive = x.IS_ACTIVE,
                GeneralFundSource = new EamisGeneralFundSourceDTO
                {
                    Name = x.GENERALFUNDSOURCE.NAME,
                },
                FinancingSource = new EamisFinancingSourceDTO
                {
                    FinancingSourceName = x.FINANCING_SOURCE.FINANCING_SOURCE_NAME
                },
                Authorization = new EamisAuthorizationDTO
                {
                    AuthorizationName = x.AUTHORIZATION.AUTHORIZATION_NAME
                }
                
            });
        }

        public async Task<EamisFundSourceDTO> Update(EamisFundSourceDTO item, int Id)
        {
            EAMISFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        private IQueryable<EAMISFUNDSOURCE> FilteredEntities(EamisFundSourceDTO filter, IQueryable<EAMISFUNDSOURCE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISFUNDSOURCE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.GenFundId != null && filter.GenFundId != 0)
                predicate = predicate.And(x => x.GENERAL_FUND_SOURCE_ID == filter.GenFundId);
            if (filter.Code != null && !string.IsNullOrEmpty(filter.Code))
                predicate = predicate.And(x => x.CODE == filter.Code);
            if (filter.AuthorizationId != null && filter.AuthorizationId != 0)
                predicate = predicate.And(x => x.AUTHORIZATION_ID == filter.AuthorizationId);
            if (filter.FinancingSourceId != null && filter.FinancingSourceId != 0)
                predicate = predicate.And(x => x.FINANCING_SOURCE_ID == filter.FinancingSourceId);
            if (filter.FundCategory != null && !string.IsNullOrEmpty(filter.FundCategory))
                predicate = predicate.And(x => x.FUND_CATEGORY == filter.FundCategory);
            var query = custom_query ?? _ctx.EAMIS_FUND_SOURCE;
            return query.Where(predicate);
        }

        public async Task<EamisFundSourceDTO> Delete(EamisFundSourceDTO item, int Id)
        {
            EAMISFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<List<EAMISFUNDSOURCE>> ListAllFundSources()
        {
            var result = await Task.Run(() => _ctx.EAMIS_FUND_SOURCE.AsNoTracking().ToList()).ConfigureAwait(false);
            return result;
        }

        public async Task<EamisFundSourceDTO> Insert(EamisFundSourceDTO item)
        {
            EAMISFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISFUNDSOURCE MapToEntity(EamisFundSourceDTO item)
        {
            if (item == null) return new EAMISFUNDSOURCE();
            return new EAMISFUNDSOURCE
            {
                ID = item.Id,
                GENERAL_FUND_SOURCE_ID = item.GenFundId,
                CODE = item.Code,
                AUTHORIZATION_ID = item.AuthorizationId,
                FINANCING_SOURCE_ID = item.FinancingSourceId,
                FUND_CATEGORY = item.FundCategory,
                IS_ACTIVE = item.IsActive
            };
        }

        public Task<bool> ValidateExistingCode(string code)
        {
            return _ctx.EAMIS_FUND_SOURCE.AsNoTracking().AnyAsync(x => x.CODE == code );
        }

        public Task<bool> UpdateValidateExistingCode(string code, int id)
        {
            return _ctx.EAMIS_FUND_SOURCE.AsNoTracking().AnyAsync(x => x.CODE == code && x.ID == id);
        }
    }
}
