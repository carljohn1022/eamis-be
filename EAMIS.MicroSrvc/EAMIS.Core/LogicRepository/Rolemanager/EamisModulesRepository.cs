using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.ContractRepository.Rolemanager;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Rolemanager
{

    public class EamisModulesRepository : IEamisModulesRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisModulesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisModulesDTO> Delete(EamisModulesDTO item)
        {
            try
            {
                EAMISMODULES data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<EamisModulesDTO> Insert(EamisModulesDTO item)
        {
            try
            {
                EAMISMODULES data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<DataList<EamisModulesDTO>> List(EamisModulesDTO filter, PageConfig config)
        {
            IQueryable<EAMISMODULES> query = FilteredEntities(filter);

            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisModulesDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        public IQueryable<EAMISMODULES> PagedQuery(IQueryable<EAMISMODULES> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisModulesDTO> Update(EamisModulesDTO item)
        {
            try
            {
                EAMISMODULES data = MapToEntity(item);
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
        private EamisModulesDTO MapToDTO(EAMISMODULES item)
        {
            if (item == null) return new EamisModulesDTO();
            return new EamisModulesDTO
            {
                Id = item.ID,
                ModuleName = item.MODULE_NAME,
                IsActive = item.IS_ACTIVE
            };

        }
        private EAMISMODULES MapToEntity(EamisModulesDTO item)
        {
            if (item == null) return new EAMISMODULES();
            return new EAMISMODULES
            {
                ID = item.Id,
                MODULE_NAME = item.ModuleName,
                IS_ACTIVE = item.IsActive
            };
        }
        private IQueryable<EamisModulesDTO> QueryToDTO(IQueryable<EAMISMODULES> query)
        {
            return query.Select(x => new EamisModulesDTO
            {
                Id = x.ID,
                ModuleName = x.MODULE_NAME,
                IsActive = x.IS_ACTIVE
            });
        }
        private IQueryable<EAMISMODULES> FilteredEntities(EamisModulesDTO filter, IQueryable<EAMISMODULES> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISMODULES>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.ModuleName)) predicate = (strict)
                     ? predicate.And(x => x.MODULE_NAME.ToLower() == filter.ModuleName.ToLower())
                     : predicate.And(x => x.MODULE_NAME.Contains(filter.ModuleName.ToLower()));
            if (filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);
            var query = custom_query ?? _ctx.EAMIS_MODULES;
            return query.Where(predicate);
        }
        public IQueryable<EAMISROLES> PagedQuery(IQueryable<EAMISROLES> query)
        {
            return query;
        }

    }
}
