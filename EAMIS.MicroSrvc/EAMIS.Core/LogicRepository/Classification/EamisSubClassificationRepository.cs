using EAMIS.Common.DTO.Classification;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Classification
{
    public class EamisSubClassificationRepository : IEamisSubClassificationRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisSubClassificationRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisSubClassificationDTO> Delete(EamisSubClassificationDTO item, int Id)
        {
            EAMISSUBCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISSUBCLASSIFICATION MapToEntity(EamisSubClassificationDTO item)
        {
            if (item == null) return new EAMISSUBCLASSIFICATION();
            return new EAMISSUBCLASSIFICATION
            {
                ID = item.Id,
                CLASSIFICATION_ID = item.ClassificationId,
                NAME_SUBCLASSIFICATION = item.NameSubClassification
            };
        }

        public async Task<EamisSubClassificationDTO> Insert(EamisSubClassificationDTO item)
        {
            EAMISSUBCLASSIFICATION data = MapToEntity(item);
            data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisSubClassificationDTO>> List(EamisSubClassificationDTO filter, PageConfig config)
        {
            IQueryable<EAMISSUBCLASSIFICATION> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisSubClassificationDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisSubClassificationDTO> QueryToDTO(IQueryable<EAMISSUBCLASSIFICATION> query)
        {
            return query.Select(x => new EamisSubClassificationDTO
            {
                Id = x.ID,
                ClassificationId = x.CLASSIFICATION_ID,
                NameSubClassification = x.NAME_SUBCLASSIFICATION,
                ClassificationDTO = new EamisClassificationDTO
                {
                    Id = x.CLASSIFICATION.ID,
                    NameClassification = x.CLASSIFICATION.NAME_CLASSIFICATION
                }
            });
        }

        private IQueryable<EAMISSUBCLASSIFICATION> PagedQuery(IQueryable<EAMISSUBCLASSIFICATION> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISSUBCLASSIFICATION> FilteredEntities(EamisSubClassificationDTO filter, IQueryable<EAMISSUBCLASSIFICATION> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSUBCLASSIFICATION>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ClassificationId != null && filter.ClassificationId != 0)
                predicate = predicate.And(x => x.CLASSIFICATION_ID == filter.ClassificationId);
            if (filter.NameSubClassification != null && !string.IsNullOrEmpty(filter.NameSubClassification))
                predicate = predicate.And(x => x.NAME_SUBCLASSIFICATION == filter.NameSubClassification);
            var query = custom_query ?? _ctx.EAMIS_SUB_CLASSIFICATION;
            return query.Where(predicate);
        }

        public async Task<EamisSubClassificationDTO> Update(EamisSubClassificationDTO item, int Id)
        {
            EAMISSUBCLASSIFICATION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
