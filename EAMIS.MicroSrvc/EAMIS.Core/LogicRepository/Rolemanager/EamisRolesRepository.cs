using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EAMIS.Core.TokenServices;
using System.Net;
using System.Configuration;
using EAMIS.Core.Response.DTO;
using EAMIS.Core.ContractRepository.Masterfiles;

namespace EAMIS.Core.LogicRepository.Masterfiles
{

    public class EamisRolesRepository : IEamisRolesRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisRolesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisRolesDTO> Delete(EamisRolesDTO item)
        {
            EAMISROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisRolesDTO> Insert(EamisRolesDTO item)
        {
            EAMISROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
                return item;
        }

        public async Task<DataList<EamisRolesDTO>> List(EamisRolesDTO filter, PageConfig config)
        {
            IQueryable<EAMISROLES> query = FilteredEntities(filter);

            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisRolesDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        public IQueryable<EAMISROLES> PagedQuery(IQueryable<EAMISROLES> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisRolesDTO> Update(EamisRolesDTO item)
        {
            EAMISROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
                return item;
        }
        private EamisRolesDTO MapToDTO(EAMISROLES item)
        {
            if (item == null) return new EamisRolesDTO();
            return new EamisRolesDTO
            {
                Id = item.ID,
                Role_Name = item.ROLE_NAME,
                IsDeleted = item.IS_DELETED

            };

        }
        private EAMISROLES MapToEntity(EamisRolesDTO item)
        {
            if (item == null) return new EAMISROLES();
            return new EAMISROLES
            {
                ID = item.Id,
                ROLE_NAME = item.Role_Name,
                IS_DELETED = item.IsDeleted
            };
        }
        private IQueryable<EamisRolesDTO> QueryToDTO(IQueryable<EAMISROLES> query)
        {
            return query.Select(x => new EamisRolesDTO
            {
                Id = x.ID,
                Role_Name = x.ROLE_NAME,
                IsDeleted = x.IS_DELETED
            });
        }
        private IQueryable<EAMISROLES> FilteredEntities(EamisRolesDTO filter, IQueryable<EAMISROLES> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISROLES>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Role_Name)) predicate = (strict)
                     ? predicate.And(x => x.ROLE_NAME.ToLower() == filter.Role_Name.ToLower())
                     : predicate.And(x => x.ROLE_NAME.Contains(filter.Role_Name.ToLower()));
            if (filter.IsDeleted != null && filter.IsDeleted != false)
                predicate = predicate.And(x => x.IS_DELETED == filter.IsDeleted);
            var query = custom_query ?? _ctx.EAMIS_ROLES;
            return query.Where(predicate);
        }
        public IQueryable<EAMISROLES> PagedQuery(IQueryable<EAMISROLES> query)
        {
            return query;
        }
    }
}
