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
    public class EamisAttachmentsRepository : IEamisAttachmentsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisAttachmentsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisAttachmentsDTO> Delete(EamisAttachmentsDTO item)
        {
            EAMISATTACHMENTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISATTACHMENTS MapToEntity(EamisAttachmentsDTO item)
        {
            if (item == null) return new EAMISATTACHMENTS();
            return new EAMISATTACHMENTS
            {
                ID = item.Id,
                ATTACHMENT_DESCRIPTION = item.AttachmentDescription,
                MODULE_NAME = item.ModuleName
               
            };
        }

        public async Task<EamisAttachmentsDTO> Insert(EamisAttachmentsDTO item)
        {
            EAMISATTACHMENTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisAttachmentsDTO>> List(EamisAttachmentsDTO filter, PageConfig config)
        {
            IQueryable<EAMISATTACHMENTS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisAttachmentsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EamisAttachmentsDTO> QueryToDTO(IQueryable<EAMISATTACHMENTS> query)
        {
            return query.Select(x => new EamisAttachmentsDTO
            {
                Id = x.ID,
                AttachmentDescription = x.ATTACHMENT_DESCRIPTION,
                AttachmentTypeDescription = x.ATTACHMENT_TYPE_DESCRIPTION,
                ModuleName = x.MODULE_NAME
                //AttachmentTypeDTO = x.ATTACHMENTTYPE.Select(y => new EamisAttachmentTypeDTO
                //{ 
                //    Id = y.ID,
                //    AttachmentId = y.ATTACHMENT_ID,
                //    AttachmentTypeDescription = y.ATTACHMENT_TYPE_DESCRIPTION
                //}).ToList()
               //AttachmentTypeDTO = new EamisAttachmentTypeDTO
               //{
               //    Id = x.ATTACHMENTTYPE.FirstOrDefault().ID,
               //    AttachmentId = x.ATTACHMENTTYPE.FirstOrDefault().ATTACHMENT_ID,
               //    AttachmentTypeDescription = x.ATTACHMENTTYPE.FirstOrDefault().ATTACHMENT_TYPE_DESCRIPTION
               //}
                
            });
        }

        public IQueryable<EAMISATTACHMENTS> PagedQuery(IQueryable<EAMISATTACHMENTS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }


        private IQueryable<EAMISATTACHMENTS> FilteredEntities(EamisAttachmentsDTO filter, IQueryable<EAMISATTACHMENTS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISATTACHMENTS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.AttachmentDescription)) predicate = (strict)
                     ? predicate.And(x => x.ATTACHMENT_DESCRIPTION.ToLower() == filter.AttachmentDescription.ToLower())
                     : predicate.And(x => x.ATTACHMENT_DESCRIPTION.Contains(filter.AttachmentDescription.ToLower()));
            if (!string.IsNullOrEmpty(filter.ModuleName)) predicate = (strict)
                      ? predicate.And(x => x.MODULE_NAME.ToLower() == filter.ModuleName.ToLower())
                      : predicate.And(x => x.MODULE_NAME.Contains(filter.ModuleName.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_ATTACHMENTS;
            return query.Where(predicate);
        }

        public async Task<EamisAttachmentsDTO> Update(EamisAttachmentsDTO item)
        {
            EAMISATTACHMENTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
