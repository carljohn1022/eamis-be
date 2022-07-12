using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Classification
{
    public class EamisGroupClassificationRepository : IEamisGroupClassificationRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisGroupClassificationRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISGROUPCLASSIFICATION>> ListGroups(string searchValue)
        {
            var result = _ctx.EAMIS_GROUP_CLASSIFICATION.AsNoTracking().Where(x => x.NAME_GROUPCLASSIFICATION == searchValue).ToList();
            return result;
        }
        public async Task<EamisGroupClassificationDTO> Delete(EamisGroupClassificationDTO item, int Id)
        {
            EAMISGROUPCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISGROUPCLASSIFICATION MapToEntity(EamisGroupClassificationDTO item)
        {
            if (item == null) return new EAMISGROUPCLASSIFICATION();
            return new EAMISGROUPCLASSIFICATION
            {
                ID = item.Id,
                CLASSIFICATION_ID = item.ClassificationId,
                SUB_CLASSIFICATION_ID = item.SubClassificationId,
                NAME_GROUPCLASSIFICATION = item.NameGroupClassification
            };
        }

        public async Task<EamisGroupClassificationDTO> Insert(EamisGroupClassificationDTO item)
        {
            EAMISGROUPCLASSIFICATION data = MapToEntity(item);
            data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisGroupClassificationDTO>> List(EamisGroupClassificationDTO filter, PageConfig config)
        {
            IQueryable<EAMISGROUPCLASSIFICATION> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisGroupClassificationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private IQueryable<EamisGroupClassificationDTO> QueryToDTO(IQueryable<EAMISGROUPCLASSIFICATION> query)
        {
            return query.Select(x => new EamisGroupClassificationDTO
            {
                Id = x.ID,
                ClassificationId = x.CLASSIFICATION_ID,
                SubClassificationId = x.SUB_CLASSIFICATION_ID,
                NameGroupClassification = x.NAME_GROUPCLASSIFICATION,
                ClassificationDTO = new EamisClassificationDTO
                {
                    Id = x.CLASSIFICATION.ID,
                    NameClassification = x.CLASSIFICATION.NAME_CLASSIFICATION
                },
                SubClassificationDTO = new EamisSubClassificationDTO
                {
                    Id = x.SUBCLASSIFICATION.ID,
                    ClassificationId = x.SUBCLASSIFICATION.CLASSIFICATION_ID,
                    NameSubClassification = x.SUBCLASSIFICATION.NAME_SUBCLASSIFICATION
                }
            });
        }

        private IQueryable<EAMISGROUPCLASSIFICATION> PagedQuery(IQueryable<EAMISGROUPCLASSIFICATION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISGROUPCLASSIFICATION> FilteredEntities(EamisGroupClassificationDTO filter, IQueryable<EAMISGROUPCLASSIFICATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISGROUPCLASSIFICATION>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ClassificationId != null && filter.ClassificationId != 0)
                predicate = predicate.And(x => x.CLASSIFICATION_ID == filter.ClassificationId);
            if (filter.SubClassificationId != null && filter.SubClassificationId != 0)
                predicate = predicate.And(x => x.SUB_CLASSIFICATION_ID == filter.SubClassificationId);
            if (filter.NameGroupClassification != null && !string.IsNullOrEmpty(filter.NameGroupClassification))
                predicate = predicate.And(x => x.NAME_GROUPCLASSIFICATION == filter.NameGroupClassification);
            var query = custom_query ?? _ctx.EAMIS_GROUP_CLASSIFICATION;
            return query.Where(predicate);
        }

        public async Task<EamisGroupClassificationDTO> Update(EamisGroupClassificationDTO item, int Id)
        {
            EAMISGROUPCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
