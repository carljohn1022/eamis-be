using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisGeneralChartofAccountsRepository : IEamisGeneralChartofAccountsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisGeneralChartofAccountsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisGeneralChartofAccountsDTO> Delete(EamisGeneralChartofAccountsDTO item, int Id)
        {
            EAMISGENERALCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISGENERALCHARTOFACCOUNTS MapToEntity(EamisGeneralChartofAccountsDTO item)
        {
            if (item == null) return new EAMISGENERALCHARTOFACCOUNTS();
            return new EAMISGENERALCHARTOFACCOUNTS
            {
                ID = item.Id,
                CLASSIFICATION = item.Classification,
                SUB_CLASSIFICATION = item.SubClassification,
                CLASSIFICATION_GROUP = item.ClassificationGroup
                
            };
        }

        public async Task<EamisGeneralChartofAccountsDTO> Insert(EamisGeneralChartofAccountsDTO item)
        {
            EAMISGENERALCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisGeneralChartofAccountsDTO>> List(EamisGeneralChartofAccountsDTO filter, PageConfig config)
        {
            IQueryable<EAMISGENERALCHARTOFACCOUNTS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisGeneralChartofAccountsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };

        }
        private IQueryable<EamisGeneralChartofAccountsDTO> QueryToDTO(IQueryable<EAMISGENERALCHARTOFACCOUNTS> query)
        {
            return query.Select(x => new EamisGeneralChartofAccountsDTO
            {
                Id = x.ID,
                Classification = x.CLASSIFICATION,
                SubClassification = x.SUB_CLASSIFICATION,
                ClassificationGroup = x.CLASSIFICATION_GROUP,
                ChartofAccountList = x.EAMIS_CHART_OF_ACCOUNTS.Select(y=> new EamisChartofAccountsDTO
                {
                    Id = y.ID,
                    GroupId = y.GROUP_ID,
                    ObjectCode = y.OBJECT_CODE,
                    AccountCode = y.ACCOUNT_CODE,
                    IsActive = y.IS_ACTIVE,
                    IsPartofInventroy = y.IS_PART_OF_INVENTORY
                    
                }).ToList(),
                
            });;
        }
        private IQueryable<EAMISGENERALCHARTOFACCOUNTS> FilteredEntities(EamisGeneralChartofAccountsDTO filter, IQueryable<EAMISGENERALCHARTOFACCOUNTS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISGENERALCHARTOFACCOUNTS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Classification)) predicate = (strict)
                     ? predicate.And(x => x.CLASSIFICATION.ToLower() == filter.Classification.ToLower())
                     : predicate.And(x => x.CLASSIFICATION.Contains(filter.Classification.ToLower()));
            if (!string.IsNullOrEmpty(filter.SubClassification)) predicate = (strict)
                     ? predicate.And(x => x.SUB_CLASSIFICATION.ToLower() == filter.SubClassification.ToLower())
                     : predicate.And(x => x.SUB_CLASSIFICATION.Contains(filter.SubClassification.ToLower()));
            if (!string.IsNullOrEmpty(filter.ClassificationGroup)) predicate = (strict)
                     ? predicate.And(x => x.CLASSIFICATION_GROUP.ToLower() == filter.ClassificationGroup.ToLower())
                     : predicate.And(x => x.CLASSIFICATION_GROUP.Contains(filter.ClassificationGroup.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_GENERAL_CHART_OF_ACCOUNTS;
            return query.Where(predicate);
        }
        private IQueryable<EAMISGENERALCHARTOFACCOUNTS> PagedQuery(IQueryable<EAMISGENERALCHARTOFACCOUNTS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisGeneralChartofAccountsDTO> Update(EamisGeneralChartofAccountsDTO item, int Id)
        {
            EAMISGENERALCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
