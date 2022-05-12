using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisPreconditionsRepository : IEamisPrecondtionsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisPreconditionsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisPreconditionsDTO> Delete(EamisPreconditionsDTO item)
        {
            EAMISPRECONDITIONS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISPRECONDITIONS MapToEntity(EamisPreconditionsDTO item)
        {
            if (item == null) return new EAMISPRECONDITIONS();
            return new EAMISPRECONDITIONS
            {
                ID = item.Id,
                PARENT_ID = item.Parent_Id,
                PRE_CONDITION_DESCRIPTION = item.Precondition_Description

            };
        }

        public async Task<EamisPreconditionsDTO> Insert(EamisPreconditionsDTO item)
        {
            EAMISPRECONDITIONS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisPreconditionsDTO>> List(EamisPreconditionsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPRECONDITIONS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPreconditionsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        //public async Task<DataList<EamisPreconditionsDTO>> ListParent(EamisPreconditionsDTO filter, PageConfig config)
        //{

        //}

        private IQueryable<EamisPreconditionsDTO> QueryToDTO(IQueryable<EAMISPRECONDITIONS> query)
        {
            return query.Select(x => new EamisPreconditionsDTO
            {
                Id = x.ID,
                Parent_Id = x.PARENT_ID,
                Precondition_Description = x.PRE_CONDITION_DESCRIPTION

            });
        }

        private IQueryable<EAMISPRECONDITIONS> PagedQuery(IQueryable<EAMISPRECONDITIONS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        private IQueryable<EAMISPRECONDITIONS> FilteredEntities(EamisPreconditionsDTO filter, IQueryable<EAMISPRECONDITIONS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPRECONDITIONS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.Parent_Id != null && filter.Parent_Id != 0)
                predicate = predicate.And(x => x.PARENT_ID == filter.Parent_Id);
            if (!string.IsNullOrEmpty(filter.Precondition_Description)) predicate = (strict)
                    ? predicate.And(x => x.PRE_CONDITION_DESCRIPTION.ToLower() == filter.Precondition_Description.ToLower())
                    : predicate.And(x => x.PRE_CONDITION_DESCRIPTION.Contains(filter.Precondition_Description.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_PPECONDITIONS;
            return query.Where(predicate);
        }

        public async Task<EamisPreconditionsDTO> Update(EamisPreconditionsDTO item)
        {
            EAMISPRECONDITIONS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
