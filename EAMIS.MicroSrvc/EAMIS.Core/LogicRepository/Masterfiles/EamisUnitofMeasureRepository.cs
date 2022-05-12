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
    public class EamisUnitofMeasureRepository : IEamisUnitofMeasureRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisUnitofMeasureRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<EamisUnitofMeasureDTO> Delete(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISUNITOFMEASURE MapToEntity(EamisUnitofMeasureDTO item)
        {
            if (item == null) return new EAMISUNITOFMEASURE();
            return new EAMISUNITOFMEASURE
            {
                ID = item.Id,
                SHORT_DESCRIPTION = item.Short_Description,
                UOM_DESCRIPTION = item.Uom_Description

            };
        }

        public async Task<EamisUnitofMeasureDTO> Insert(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisUnitofMeasureDTO>> List(EamisUnitofMeasureDTO filter, PageConfig config)
        {
            IQueryable<EAMISUNITOFMEASURE> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisUnitofMeasureDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        private IQueryable<EamisUnitofMeasureDTO> QueryToDTO(IQueryable<EAMISUNITOFMEASURE> query)
        {
            return query.Select(x => new EamisUnitofMeasureDTO
            {
                Id = x.ID,
                Short_Description = x.SHORT_DESCRIPTION,
                Uom_Description = x.UOM_DESCRIPTION

            });
        }

        private IQueryable<EAMISUNITOFMEASURE> PagedQuery(IQueryable<EAMISUNITOFMEASURE> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }

        private IQueryable<EAMISUNITOFMEASURE> FilteredEntities(EamisUnitofMeasureDTO filter, IQueryable<EAMISUNITOFMEASURE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISUNITOFMEASURE>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (!string.IsNullOrEmpty(filter.Uom_Description)) predicate = (strict)
                    ? predicate.And(x => x.UOM_DESCRIPTION.ToLower() == filter.Uom_Description.ToLower())
                    : predicate.And(x => x.UOM_DESCRIPTION.Contains(filter.Uom_Description.ToLower()));
            var query = custom_query ?? _ctx.EAMIS_UNITOFMEASURE;
            return query.Where(predicate);
        }

        public async Task<EamisUnitofMeasureDTO> Update(EamisUnitofMeasureDTO item)
        {
            EAMISUNITOFMEASURE data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
