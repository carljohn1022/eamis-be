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
    public class EamisNoticeofDeliveryRepository : IEamisNoticeofDeliveryRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisNoticeofDeliveryRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisNoticeofDeliveryDTO> Delete(EamisNoticeofDeliveryDTO item)
        {
            EAMISNOTICEOFDELIVERY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisNoticeofDeliveryDTO> Insert(EamisNoticeofDeliveryDTO item)
        {
            EAMISNOTICEOFDELIVERY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisNoticeofDeliveryDTO> Update(EamisNoticeofDeliveryDTO item)
        {
            EAMISNOTICEOFDELIVERY data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisNoticeofDeliveryDTO>> List(EamisNoticeofDeliveryDTO filter,PageConfig config)
        {
            IQueryable<EAMISNOTICEOFDELIVERY> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_IsAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisNoticeofDeliveryDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }
        private IQueryable<EamisNoticeofDeliveryDTO> QueryToDTO(IQueryable<EAMISNOTICEOFDELIVERY> query)
        {
            return query.Select(x => new EamisNoticeofDeliveryDTO
            {
                Id = x.ID,
                UserId = x.USER_ID,
               TransactioId = x.TRANSACTION_ID,
               PropertyDetails_Id = x.PROPERTY_DETAILS_ID,
               PurchaseOrderNo = x.PURCHASE_REQUEST_NO,
               PurchaseRequestNo = x.PURCHASE_REQUEST_NO,
               InspectionType = x.INSPECTION_TYPE,
               DeliveryDate = x.DELIVERY_DATE,
               IsWrongProperty = x.IS_WRONG_PROPERTY,
               IsInCompleteProperty = x.IS_INCOMPLETE_PROPERTY,
               IsWaranttyCertificate = x.IS_WARRANTY_CERTIFICATE,
               IsWaterMaterial = x.IS_WATER_MATERAIL,

            });
        }

        private IQueryable<EAMISNOTICEOFDELIVERY> PagedQuery(IQueryable<EAMISNOTICEOFDELIVERY> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);

        }
        private EAMISNOTICEOFDELIVERY MapToEntity(EamisNoticeofDeliveryDTO item)
        {
            if (item == null) return new EAMISNOTICEOFDELIVERY();
            return new EAMISNOTICEOFDELIVERY
            {
                ID = item.Id,
                USER_ID = item.UserId,
                TRANSACTION_ID = item.TransactioId,
                PROPERTY_DETAILS_ID = item.PropertyDetails_Id,
                PURCHASE_REQUEST_NO = item.PurchaseRequestNo,
                PUCHASE_ORDER_NO = item.PurchaseOrderNo,
                INSPECTION_TYPE = item.InspectionType,
                DELIVERY_DATE = item.DeliveryDate,
                IS_INCOMPLETE_PROPERTY = item.IsInCompleteProperty,
                IS_WARRANTY_CERTIFICATE = item.IsWaranttyCertificate,
                IS_WATER_MATERAIL = item.IsWaterMaterial,
                IS_WRONG_PROPERTY = item.IsWrongProperty,


            };
        }

        private IQueryable<EAMISNOTICEOFDELIVERY> FilteredEntities(EamisNoticeofDeliveryDTO filter, IQueryable<EAMISNOTICEOFDELIVERY> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISNOTICEOFDELIVERY>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.TransactioId != null && filter.TransactioId != 0)
                predicate = predicate.And(x => x.TRANSACTION_ID == filter.TransactioId);
            if (filter.UserId != null && filter.UserId != 0)
                predicate = predicate.And(x => x.USER_ID == filter.UserId);
            if (filter.PropertyDetails_Id != null && filter.PropertyDetails_Id != 0)
                predicate = predicate.And(x => x.PROPERTY_DETAILS_ID == filter.PropertyDetails_Id);
            if (filter.PurchaseOrderNo != null && filter.PurchaseOrderNo != 0)
                predicate = predicate.And(x => x.PUCHASE_ORDER_NO == filter.PurchaseOrderNo);
            if (filter.PurchaseRequestNo != null && filter.PurchaseRequestNo != 0)
                predicate = predicate.And(x => x.PUCHASE_ORDER_NO == filter.PurchaseRequestNo);
            if (filter.InspectionType != null && string.IsNullOrEmpty(filter.InspectionType))
                predicate = predicate.And(x => x.INSPECTION_TYPE == filter.InspectionType);
            if (filter.IsInCompleteProperty != null && filter.IsInCompleteProperty != false)
                predicate = predicate.And(x => x.IS_INCOMPLETE_PROPERTY == filter.IsInCompleteProperty);
            if (filter.IsWaranttyCertificate != null && filter.IsWaranttyCertificate != false)
                predicate = predicate.And(x => x.IS_WARRANTY_CERTIFICATE == filter.IsWaranttyCertificate);
            if (filter.IsWaterMaterial != null && filter.IsWaterMaterial != false)
                predicate = predicate.And(x => x.IS_WATER_MATERAIL == filter.IsWaterMaterial);
            if (filter.IsWrongProperty != null && filter.IsWrongProperty != false)
                predicate = predicate.And(x => x.IS_WRONG_PROPERTY == filter.IsWrongProperty);
                     var query = custom_query ?? _ctx.EAMIS_NOTICE_OF_DELIVERY;
            return query.Where(predicate);
        }

      
    }
}
