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
    public class AisPersonnelRepository : IAisPersonnelRepository
    {
        AISContext _aisctx;
        private readonly int _maxPageSize;

        public AisPersonnelRepository(AISContext aisctx)
        {
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString()   );
        }

        public async Task<DataList<AisPersonnelDTO>> List(AisPersonnelDTO filter, PageConfig config)
        {
            IQueryable<AISPERSONNEL> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<AisPersonnelDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<AISPERSONNEL> FilteredEntities(AisPersonnelDTO filter, IQueryable<AISPERSONNEL> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<AISPERSONNEL>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.Id == filter.Id);
            if (filter.OfficeId != null && filter.OfficeId != 0)
                predicate = predicate.And(x => x.OfficeId == filter.OfficeId);
            if (filter.AgencyEmployeeNumber != null && !string.IsNullOrEmpty(filter.AgencyEmployeeNumber))
                predicate = predicate.And(x => x.AgencyEmployeeNumber == filter.AgencyEmployeeNumber);
            if (filter.SexId != null && filter.SexId != 0)
                predicate = predicate.And(x => x.SexId == filter.SexId);
            if (filter.LastName != null && !string.IsNullOrEmpty(filter.LastName))
                predicate = predicate.And(x => x.LastName == filter.LastName);
            if (filter.FirstName != null && !string.IsNullOrEmpty(filter.FirstName))
                predicate = predicate.And(x => x.FirstName == filter.FirstName);
            if (filter.MiddleName != null && !string.IsNullOrEmpty(filter.MiddleName))
                predicate = predicate.And(x => x.MiddleName == filter.MiddleName);
            if (filter.ExtensionName != null && !string.IsNullOrEmpty(filter.ExtensionName))
                predicate = predicate.And(x => x.ExtensionName == filter.ExtensionName);
            if (filter.NickName != null && !string.IsNullOrEmpty(filter.NickName))
                predicate = predicate.And(x => x.NickName == filter.NickName);
            if (filter.ProfilePhoto != null && !string.IsNullOrEmpty(filter.ProfilePhoto))
                predicate = predicate.And(x => x.ProfilePhoto == filter.ProfilePhoto);
            if (filter.isDeleted != null && filter.isDeleted != false)
                predicate = predicate.And(x => x.isDeleted == filter.isDeleted);
            var query = custom_query ?? _aisctx.Personnel;
            return query.Where(predicate);
        }

        public IQueryable<AISPERSONNEL> PagedQuery(IQueryable<AISPERSONNEL> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<AisPersonnelDTO> QueryToDTO(IQueryable<AISPERSONNEL> query)
        {
            return query.Select(x => new AisPersonnelDTO
            {
                Id = x.Id,
                OfficeId = x.OfficeId,
                AgencyEmployeeNumber = x.AgencyEmployeeNumber,
                SexId = x.SexId,
                LastName = x.LastName,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                ExtensionName = x.ExtensionName,
                NickName = x.NickName,
                ProfilePhoto = x.ProfilePhoto,
                isDeleted = x.isDeleted,
            });
        }
        private AisPersonnelDTO MapToDTO(AISPERSONNEL item)
        {
            if (item == null) return null;
            return new AisPersonnelDTO
            {
                Id = item.Id,
                OfficeId = item.OfficeId,
                AgencyEmployeeNumber = item.AgencyEmployeeNumber,
                SexId = item.SexId,
                LastName = item.LastName,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                ExtensionName = item.ExtensionName,
                NickName = item.NickName,
                ProfilePhoto = item.ProfilePhoto,
                isDeleted = item.isDeleted,
            };
        }
        private AISPERSONNEL MapToEntity(AisPersonnelDTO item)
        {
            if (item == null) return new AISPERSONNEL();
            return new AISPERSONNEL
            {
                Id = item.Id,
                OfficeId = item.OfficeId,
                AgencyEmployeeNumber = item.AgencyEmployeeNumber,
                SexId = item.SexId,
                LastName = item.LastName,
                FirstName = item.FirstName,
                MiddleName = item.MiddleName,
                ExtensionName = item.ExtensionName,
                NickName = item.NickName,
                ProfilePhoto = item.ProfilePhoto,
                isDeleted = item.isDeleted,
            };
        }

        public async Task<AisPersonnelDTO> GetPersonnelByAgencyEmployeeId(string AgencyEmployeeNumber)
        {
            return await _aisctx.Personnel.AsNoTracking().Where(x => x.AgencyEmployeeNumber == AgencyEmployeeNumber).Select(y=> new AisPersonnelDTO 
            {
                Id = y.Id,
                OfficeId = y.OfficeId,
                AgencyEmployeeNumber = y.AgencyEmployeeNumber,
                SexId = y.SexId,
                LastName = y.LastName,
                FirstName = y.FirstName,
                MiddleName = y.MiddleName,
                ExtensionName = y.ExtensionName,
                NickName = y.NickName,
                ProfilePhoto = y.ProfilePhoto,
                isDeleted = y.isDeleted,
            }).FirstOrDefaultAsync();
        }
    }
}
