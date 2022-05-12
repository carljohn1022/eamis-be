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
    public class AisOfficeRepository : IAisOfficeRepository
    {
        private AISContext _aisctx;
        private readonly int _maxPageSize;

        public AisOfficeRepository(AISContext aisctx)
        {
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<AisOfficeDTO>> List(AisOfficeDTO item, PageConfig config)
        {
            IQueryable<AISOFFICE> query = FilteredEntities(item);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<AisOfficeDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<AISOFFICE> FilteredEntities(AisOfficeDTO filter, IQueryable<AISOFFICE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<AISOFFICE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.Id == filter.Id);
            if (filter.OfficeTypeId != null && filter.OfficeTypeId != 0)
                predicate = predicate.And(x => x.OfficeTypeId == filter.OfficeTypeId);
            if (filter.OrgCode != null && !string.IsNullOrEmpty(filter.OrgCode))
                predicate = predicate.And(x => x.OrgCode == filter.OrgCode);
            if (filter.ParentOfficeId != null && filter.ParentOfficeId != 0)
                predicate = predicate.And(x => x.ParentOfficeId == filter.ParentOfficeId);
            if (filter.LongName != null && !string.IsNullOrEmpty(filter.LongName))
                predicate = predicate.And(x => x.LongName == filter.LongName);
            if (filter.ShortName != null && !string.IsNullOrEmpty(filter.ShortName))
                predicate = predicate.And(x => x.ShortName == filter.ShortName);
            if (filter.Address != null && !string.IsNullOrEmpty(filter.Address))
                predicate = predicate.And(x => x.Address == filter.Address);
            var query = custom_query ?? _aisctx.Office;
            return query.Where(predicate);
        }

        public IQueryable<AISOFFICE> PagedQuery(IQueryable<AISOFFICE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<AisOfficeDTO> QueryToDTO(IQueryable<AISOFFICE> query)
        {
            return query.Select(x => new AisOfficeDTO
            {
                Id = x.Id,
                OfficeTypeId = x.OfficeTypeId,
                ParentOfficeId = x.ParentOfficeId ?? 0,
                OrgCode = x.OrgCode ?? null,
                ShortName = x.ShortName,
                LongName = x.LongName,
                Address = x.Address ?? null,
                
            });
        }
        private AisOfficeDTO MapToDTO(AISOFFICE item)
        {
            if (item == null) return null;
            return new AisOfficeDTO
            {
                Id = item.Id,
                OfficeTypeId = item.OfficeTypeId,
                ParentOfficeId = item.ParentOfficeId ?? 0,
                OrgCode = item.OrgCode ?? null,
                ShortName = item.ShortName,
                LongName = item.LongName,
                Address = item.Address ?? null,
            };
        }
        private AISOFFICE MapToEntity(AisOfficeDTO item)
        {
            if (item == null) return new AISOFFICE();
            return new AISOFFICE
            {
                Id = item.Id,
                OfficeTypeId = item.OfficeTypeId,
                ParentOfficeId = item.ParentOfficeId,
                OrgCode = item.OrgCode,
                ShortName = item.ShortName,
                LongName = item.LongName,
                Address = item.Address,
            };
        }
    }
}
