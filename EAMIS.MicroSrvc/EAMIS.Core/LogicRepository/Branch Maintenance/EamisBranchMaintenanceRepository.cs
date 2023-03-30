using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Branch_Maintenance;
using EAMIS.Core.ContractRepository.Branch_Maintenance;
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

namespace EAMIS.Core.LogicRepository.Branch_Maintenance
{
    public class EamisBranchMaintenanceRepository : IEamisBranchMaintenanceRepository
    {
        private readonly EAMISContext _ctx;
        private readonly AISContext _aisctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisBranchMaintenanceRepository(EAMISContext ctx, AISContext aisctx)
        {
            _ctx = ctx;
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisBranchDTO> Delete(EamisBranchDTO item)
        {
            EAMISBRANCHMAINTENANCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISBRANCHMAINTENANCE MapToEntity(EamisBranchDTO item)
        {
            //var chartAccount = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().ToList();

            if (item == null) return new EAMISBRANCHMAINTENANCE();
            return new EAMISBRANCHMAINTENANCE
            {
                SeqID = item.SeqID,
                BranchID = item.BranchID,
                BranchDescription = item.BranchDescription,
                Region = item.Region,
                AreaID = item.AreaID,
                AreaDescription = item.AreaDescription,
            };
        }

        public async Task<EamisBranchDTO> Insert(EamisBranchDTO item)
        {
            EAMISBRANCHMAINTENANCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<EamisBranchDTO> Update(int seqID, EamisBranchDTO item)
        {
            EAMISBRANCHMAINTENANCE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<DataList<EamisBranchDTO>> List(EamisBranchDTO filter, PageConfig config)
        {
            IQueryable<EAMISBRANCHMAINTENANCE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisBranchDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EAMISBRANCHMAINTENANCE> PagedQuery(IQueryable<EAMISBRANCHMAINTENANCE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.SeqID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisBranchDTO> QueryToDTO(IQueryable<EAMISBRANCHMAINTENANCE> query)
        {
            return query.Select(x => new EamisBranchDTO
            {
                SeqID = x.SeqID,
                BranchID = x.BranchID,
                BranchDescription = x.BranchDescription,
                Region = x.Region,
                AreaID = x.AreaID,
                AreaDescription = x.AreaDescription,
                RegionGroup = _ctx.EAMIS_REGION.AsNoTracking().Where(h => h.REGION_CODE.ToString() == x.Region)
                                            .Select(h => new EamisRegionDTO
                                            {
                                                RegionCode = h.REGION_CODE,
                                                RegionDescription = h.REGION_DESCRIPTION
                                            }).FirstOrDefault()
            });
        }

        private IQueryable<EAMISBRANCHMAINTENANCE> FilteredEntities(EamisBranchDTO filter, IQueryable<EAMISBRANCHMAINTENANCE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISBRANCHMAINTENANCE>(true);
            if (filter.SeqID != null && filter.SeqID != 0)
                predicate = predicate.And(x => x.SeqID == filter.SeqID);

            if (!string.IsNullOrEmpty(filter.BranchID)) predicate = (strict)
                    ? predicate.And(x => x.BranchID.ToLower() == filter.BranchID.ToLower())
                    : predicate.And(x => x.BranchID.ToLower() == filter.BranchID.ToLower());

            if (!string.IsNullOrEmpty(filter.BranchDescription)) predicate = (strict)
                    ? predicate.And(x => x.BranchDescription.ToLower() == filter.BranchDescription.ToLower())
                    : predicate.And(x => x.BranchDescription.ToLower() == filter.BranchDescription.ToLower());

            if (!string.IsNullOrEmpty(filter.Region)) predicate = (strict)
                    ? predicate.And(x => x.Region.ToLower() == filter.Region.ToLower())
                    : predicate.And(x => x.Region.ToLower() == filter.Region.ToLower());

            if (!string.IsNullOrEmpty(filter.AreaID)) predicate = (strict)
                   ? predicate.And(x => x.AreaID.ToLower() == filter.AreaID.ToLower())
                   : predicate.And(x => x.AreaID.ToLower() == filter.AreaID.ToLower());

            if (!string.IsNullOrEmpty(filter.AreaDescription)) predicate = (strict)
                   ? predicate.And(x => x.AreaDescription.ToLower() == filter.AreaDescription.ToLower())
                   : predicate.And(x => x.AreaDescription.ToLower() == filter.AreaDescription.ToLower());

            var query = custom_query ?? _ctx.branch;
            return query.Where(predicate);
        }
        public Task<bool> ValidateExistingBranchID(string branchID)
        {
            return _ctx.branch.AsNoTracking().AnyAsync(x => x.BranchID == branchID);
        }

        public Task<bool> ValidateExistingBranchUpdate(int seqID, string branchID)
        {
            return _ctx.branch.AsNoTracking().AnyAsync(x => x.SeqID == seqID && x.BranchID == branchID);
        }
    }
}
