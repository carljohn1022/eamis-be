using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
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

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyDetailsRepository : IEamisPropertyDetailsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisPropertyDetailsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
            : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisPropertyDetailsDTO> Delete(EamisPropertyDetailsDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisPropertyDetailsDTO> Insert(EamisPropertyDetailsDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisPropertyDetailsDTO>> List(EamisPropertyDetailsDTO filter,PageConfig config)
        {
            IQueryable<EAMISPROPERTYDETAILS> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_IsAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyDetailsDTO>()
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<EAMISPROPERTYDETAILS> PagedQuery(IQueryable<EAMISPROPERTYDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<EamisPropertyDetailsDTO> Update(EamisPropertyDetailsDTO item)
        {
            EAMISPROPERTYDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
           await _ctx.SaveChangesAsync();
            return item;
        }
        private EAMISPROPERTYDETAILS MapToEntity(EamisPropertyDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYDETAILS();
            return new EAMISPROPERTYDETAILS
            {
                ID = item.Id,
                BRAND = item.Brand,
                PROPERTY_NAME = item.PropertyName,
                MODEL_NO = item.ModelNo,
                SERIAL_NO = item.SerialNo,
                UNIT_COST = item.UnitCost,
                SOURCE_ID = item.SourceId,
                STOCK_NO = item.StockNo,
                QTY_IN_STOCK = item.QtyInStock,
                PROPERTY_TYPE_ID = item.PropertyTypeId,
                IS_PROPERTY = item.IsProperty,
                IS_STOCKABLE = item.IsStockable,
                UOM_ID = item.UomId,
            };
        }
        private IQueryable<EamisPropertyDetailsDTO> QueryToDTO(IQueryable<EAMISPROPERTYDETAILS> query)
        {
            return query.Select(x => new EamisPropertyDetailsDTO
            {
                Id = x.ID,
                Brand = x.BRAND,
                PropertyName = x.PROPERTY_NAME,
                ModelNo = x.MODEL_NO,
                SerialNo = x.SERIAL_NO,
                UnitCost = x.UNIT_COST,
                SourceId = x.SOURCE_ID,
                PropertyTypeId =x.PROPERTY_TYPE_ID,
                QtyInStock = x.QTY_IN_STOCK,
                UomId = x.UOM_ID,
                StockNo = x.STOCK_NO,
                IsProperty =x.IS_PROPERTY,
                IsStockable = x.IS_STOCKABLE,

            });
        }
        private IQueryable<EAMISPROPERTYDETAILS> FilteredEntities(EamisPropertyDetailsDTO filter, IQueryable<EAMISPROPERTYDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYDETAILS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.Brand != null && !string.IsNullOrEmpty(filter.Brand))
                predicate = predicate.And(x => x.BRAND == filter.Brand);
            if (filter.PropertyName != null && !string.IsNullOrEmpty(filter.PropertyName))
                predicate = predicate.And(x => x.PROPERTY_NAME == filter.PropertyName);
            if (filter.PropertyTypeId != null && filter.PropertyTypeId != 0)
                predicate = predicate.And(x => x.PROPERTY_TYPE_ID == filter.PropertyTypeId);
            if (filter.ModelNo != null && !string.IsNullOrEmpty(filter.ModelNo))
                predicate = predicate.And(x => x.MODEL_NO == filter.ModelNo);
            if (filter.SerialNo != null && !string.IsNullOrEmpty(filter.SerialNo))
                predicate = predicate.And(x => x.SERIAL_NO == filter.SerialNo);
            if (filter.StockNo != null && filter.StockNo != 0)
                predicate = predicate.And(x => x.STOCK_NO == filter.StockNo);
            if (filter.UnitCost != null && filter.UnitCost != 0)
                predicate = predicate.And(x => x.UNIT_COST == filter.UnitCost);
            if (filter.SourceId != null && filter.SourceId != 0)
                predicate = predicate.And(x => x.SOURCE_ID == filter.SourceId);
            if (filter.QtyInStock != null && filter.QtyInStock != 0)
                predicate = predicate.And(x => x.QTY_IN_STOCK == filter.QtyInStock);
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_DETAILS;
            return query.Where(predicate);
        }
    }
}
