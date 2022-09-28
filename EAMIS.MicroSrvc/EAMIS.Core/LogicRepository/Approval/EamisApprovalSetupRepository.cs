using EAMIS.Common.DTO.Approval;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Approval
{
    public class EamisApprovalSetupRepository : IEamisApprovalSetupRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisApprovalSetupRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<EamisApprovalSetupDTO>> List(EamisApprovalSetupDTO filter, PageConfig config)
        {
            IQueryable<EAMISAPPROVALSETUP> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisApprovalSetupDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisApprovalSetupDTO> QueryToDTO(IQueryable<EAMISAPPROVALSETUP> query)
        {
            return query.Select(x => new EamisApprovalSetupDTO
            {
                Id = x.ID,
                ModuleName = x.MODULE_NAME,
                MaxLevel = x.MAX_LEVEL
            });
        }

        private IQueryable<EAMISAPPROVALSETUP> FilteredEntities(EamisApprovalSetupDTO filter, IQueryable<EAMISAPPROVALSETUP> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISAPPROVALSETUP>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ModuleName != null && !string.IsNullOrEmpty(filter.ModuleName))
                predicate = predicate.And(x => x.MODULE_NAME == filter.ModuleName);
            if (filter.MaxLevel != 0)
                predicate = predicate.And(x => x.MAX_LEVEL == filter.MaxLevel);
            var query = custom_query ?? _ctx.EAMIS_APPROVAL_SETUP;
            return query.Where(predicate);
        }

        public IQueryable<EAMISAPPROVALSETUP> PagedQuery(IQueryable<EAMISAPPROVALSETUP> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisApprovalSetupDTO> Insert(EamisApprovalSetupDTO item)
        {
            try
            {
                EAMISAPPROVALSETUP data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        private EAMISAPPROVALSETUP MapToEntity(EamisApprovalSetupDTO item)
        {
            if (item == null) return new EAMISAPPROVALSETUP();
            return new EAMISAPPROVALSETUP
            {
                ID = item.Id,
                MODULE_NAME = item.ModuleName,
                MAX_LEVEL = item.MaxLevel
            };
        }

        public async Task<EamisApprovalSetupDTO> Update(EamisApprovalSetupDTO item)
        {
            try
            {
                EAMISAPPROVALSETUP data = MapToEntity(item);
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

        public async Task<EamisApprovalSetupDTO> Delete(EamisApprovalSetupDTO item)
        {
            try
            {
                EAMISAPPROVALSETUP data = MapToEntity(item);
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
    }
}
