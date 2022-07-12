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
    public class EamisAuthorizationRepository : IEamisAuthorizationRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisAuthorizationRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISAUTHORIZATION>> ListAuthorization(string searchValue)
        {
            var result = _ctx.EAMIS_AUTHORIZATION.AsNoTracking().Where(x => x.AUTHORIZATION_NAME == searchValue).ToList();
            return result;
        }
        public async Task<DataList<EamisAuthorizationDTO>> List(EamisAuthorizationDTO filter, PageConfig config)
        {
            IQueryable<EAMISAUTHORIZATION> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisAuthorizationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EamisAuthorizationDTO> QueryToDTO(IQueryable<EAMISAUTHORIZATION> query)
        {
            return query.Select(x => new EamisAuthorizationDTO
            {
                Id = x.ID,
                AuthorizationName = x.AUTHORIZATION_NAME
            });
        }

        private IQueryable<EAMISAUTHORIZATION> PagedQuery(IQueryable<EAMISAUTHORIZATION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISAUTHORIZATION> FilteredEntities(EamisAuthorizationDTO filter, IQueryable<EAMISAUTHORIZATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISAUTHORIZATION>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (!string.IsNullOrEmpty(filter.AuthorizationName)) predicate = (strict)
                    ? predicate.And(x => x.AUTHORIZATION_NAME.ToLower() == filter.AuthorizationName.ToLower())
                    : predicate.And(x => x.AUTHORIZATION_NAME.ToLower() == filter.AuthorizationName.ToLower());
            var query = custom_query ?? _ctx.EAMIS_AUTHORIZATION;
            return query.Where(predicate);
        }
    }
}
