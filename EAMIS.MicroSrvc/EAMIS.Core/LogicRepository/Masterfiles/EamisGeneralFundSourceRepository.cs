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
    public class EamisGeneralFundSourceRepository : IEamisGeneralFundSourceRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisGeneralFundSourceRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                           : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        private EAMISGENERALFUNDSOURCE MapToEntity(EamisGeneralFundSourceDTO item)
        {
            if (item == null) return new EAMISGENERALFUNDSOURCE();
            return new EAMISGENERALFUNDSOURCE
            {
                ID = item.Id,
                NAME = item.Name
               
            };
        }
        public async Task<EamisGeneralFundSourceDTO> Delete(EamisGeneralFundSourceDTO item, int Id)
        {
            EAMISGENERALFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisGeneralFundSourceDTO> Insert(EamisGeneralFundSourceDTO item)
        {
            EAMISGENERALFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisGeneralFundSourceDTO>> List(EamisGeneralFundSourceDTO filter, PageConfig config)
        {
            IQueryable<EAMISGENERALFUNDSOURCE> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisGeneralFundSourceDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<EAMISGENERALFUNDSOURCE> PagedQuery(IQueryable<EAMISGENERALFUNDSOURCE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EAMISGENERALFUNDSOURCE> FilteredEntities(EamisGeneralFundSourceDTO filter, IQueryable<EAMISGENERALFUNDSOURCE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISGENERALFUNDSOURCE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.Name != null && !string.IsNullOrEmpty(filter.Name))
                predicate = predicate.And(x => x.NAME == filter.Name);
            var query = custom_query ?? _ctx.EAMIS_GENERAL_FUND_SOURCE;
            return query.Where(predicate);
        }
        private IQueryable<EamisGeneralFundSourceDTO> QueryToDTO(IQueryable<EAMISGENERALFUNDSOURCE> query)
        {
            return query.Select(x => new EamisGeneralFundSourceDTO
            {
                Id = x.ID,
                Name = x.NAME,
                //FundSource = x.FUNDSOURCE.Select(e=> new EamisFundSourceDTO { 
                //Id = e.ID,
                //GenFundId = e.GENERAL_FUND_SOURCE_ID,
                //Code = e.CODE,
                //AuthorizationId = e.AUTHORIZATION_ID,
                //FinancingSourceId = e.FINANCING_SOURCE_ID,
                //FundCategory = e.FUND_CATEGORY,
                //}).ToList(),
            });
        }


        public async Task<EamisGeneralFundSourceDTO> Update(EamisGeneralFundSourceDTO item, int Id)
        {
            EAMISGENERALFUNDSOURCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
       
    }

      
}
