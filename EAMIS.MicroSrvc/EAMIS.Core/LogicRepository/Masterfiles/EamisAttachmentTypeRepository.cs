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
    public class EamisAttachmentTypeRepository : IEamisAttachmentTypeRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisAttachmentTypeRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisAttachmentTypeDTO> Delete(EamisAttachmentTypeDTO item)
        {
            EAMISATTACHMENTTYPE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISATTACHMENTTYPE MapToEntity(EamisAttachmentTypeDTO item)
        {
            if (item == null) return new EAMISATTACHMENTTYPE();
            return new EAMISATTACHMENTTYPE
            {
                ID = item.Id,
                ATTACHMENT_ID = item.AttachmentId,
                ATTACHMENT_TYPE_DESCRIPTION = item.AttachmentTypeDescription
            };
        }

        public async Task<EamisAttachmentTypeDTO> Insert(EamisAttachmentTypeDTO item)
        {
            EAMISATTACHMENTTYPE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisAttachmentTypeDTO>> List(EamisAttachmentTypeDTO filter, PageConfig config)
        {
            IQueryable<EAMISATTACHMENTTYPE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisAttachmentTypeDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisAttachmentTypeDTO> QueryToDTO(IQueryable<EAMISATTACHMENTTYPE> query)
        {
            return query.Select(x => new EamisAttachmentTypeDTO
            {
                Id = x.ID,
                AttachmentId = x.ATTACHMENT_ID,
                AttachmentTypeDescription =  x.ATTACHMENT_TYPE_DESCRIPTION,
                EamisAttachmentsDTO = new EamisAttachmentsDTO
                {
                    Id = x.ATTACHMENTS.ID,
                    AttachmentDescription = x.ATTACHMENTS.ATTACHMENT_DESCRIPTION,
                    ModuleName = x.ATTACHMENTS.MODULE_NAME
                }

            });
        }

        public IQueryable<EAMISATTACHMENTTYPE> PagedQuery(IQueryable<EAMISATTACHMENTTYPE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISATTACHMENTTYPE> FilteredEntities(EamisAttachmentTypeDTO filter, IQueryable<EAMISATTACHMENTTYPE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISATTACHMENTTYPE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.AttachmentId != null && filter.AttachmentId != 0)
                predicate = predicate.And(x => x.ID == filter.AttachmentId);
            if (!string.IsNullOrEmpty(filter.AttachmentTypeDescription)) predicate = (strict)
                     ? predicate.And(x => x.ATTACHMENT_TYPE_DESCRIPTION.ToLower() == filter.AttachmentTypeDescription.ToLower())
                     : predicate.And(x => x.ATTACHMENT_TYPE_DESCRIPTION.Contains(filter.AttachmentTypeDescription.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_ATTACHMENT_TYPE;
            return query.Where(predicate);
        }

        public async Task<EamisAttachmentTypeDTO> Update(EamisAttachmentTypeDTO item)
        {
            EAMISATTACHMENTTYPE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
