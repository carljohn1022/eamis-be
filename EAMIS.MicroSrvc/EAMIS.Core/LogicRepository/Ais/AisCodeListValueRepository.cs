using EAMIS.Common.DTO.Ais;
using EAMIS.Core.ContractRepository.Ais;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities.AIS;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Ais
{
    public class AisCodeListValueRepository : IAisCodeListValueRepository
    {
        AISContext _aisctx;
        private readonly int _maxPageSize;
        public AisCodeListValueRepository(AISContext aisctx)
        {
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<AisCodeListValueDTO>> List(AisCodeListValueDTO item, PageConfig config)
        {
            IQueryable<AISCODELISTVALUE> query = FilteredEntities(item);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<AisCodeListValueDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<AISCODELISTVALUE> FilteredEntities(AisCodeListValueDTO filter, IQueryable<AISCODELISTVALUE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<AISCODELISTVALUE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.Id == filter.Id);
            if (filter.CodeListType != null && !string.IsNullOrEmpty(filter.CodeListType))
                predicate = predicate.And(x => x.CodeListType == filter.CodeListType);
            if (filter.Code != null && !string.IsNullOrEmpty(filter.Code))
                predicate = predicate.And(x => x.Code == filter.Code);
            if (filter.Name != null && !string.IsNullOrEmpty(filter.Name))
                predicate = predicate.And(x => x.Name == filter.Name);
            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IsActive);
            if (filter.IsDeleted != null && filter.IsDeleted != false)
                predicate = predicate.And(x => x.IsDeleted);
            var query = custom_query ?? _aisctx.CodeListValue;
            return query.Where(predicate);
        }

        public IQueryable<AISCODELISTVALUE> PagedQuery(IQueryable<AISCODELISTVALUE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<AisCodeListValueDTO> QueryToDTO(IQueryable<AISCODELISTVALUE> query)
        {
            return query.Select(x => new AisCodeListValueDTO
            {
                Id = x.Id,
               CodeListType = x.CodeListType,
               Code = x.Code,
               Name = x.Name,
               IsActive = x.IsActive,
               IsDeleted = x.IsDeleted

            });
        }
        private AisCodeListValueDTO MapToDTO(AISCODELISTVALUE item)
        {
            if (item == null) return null;
            return new AisCodeListValueDTO
            {
                Id = item.Id,
                CodeListType = item.CodeListType,
                Code = item.Code,
                Name = item.Name,
                IsActive = item.IsActive,
                IsDeleted = item.IsDeleted
            };
        }
        private AISCODELISTVALUE MapToEntity(AisCodeListValueDTO item)
        {
            if (item == null) return new AISCODELISTVALUE();
            return new AISCODELISTVALUE
            {
                Id = item.Id,
                CodeListType = item.CodeListType,
                Code = item.Code,
                Name = item.Name,
                IsActive = item.IsActive,
                IsDeleted = item.IsDeleted
            };
        }
    }
}
