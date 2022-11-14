using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
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
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyLedgerRepository : IEamisPropertyLedgerRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyLedgerRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        private EAMISPROPERTYLEDGER MapToEntity(EamisPropertyLedgerDTO item)
        {
            if (item == null) return new EAMISPROPERTYLEDGER();
            return new EAMISPROPERTYLEDGER
            {
                ID = item.Id,
                AS_OF_DATE = item.AsOfDate,
                ITEM_CODE = item.ItemCode,
                ITEM_DESC = item.ItemDesc,
                PHYSICAL_COUNT = item.PhysicalCounnt,
                REMARKS = item.Remarks,
                RESPONSIBILITY_CODE = item.ResponsibilityCode,
                TOTAL_ISSUED = item.TotalIssued,
                TOTAL_ON_HAND_DR = item.TotalOnHandDR,
                TOTAL_RECEIVED = item.TotalReceived,
                TOTAL_VALUE_OH = item.TotalValueOH,
                TOTAL_VALUE_PC = item.TotalValuePC,
                TOTAL_VALUE_VAR = item.TotalValueVar,
                UNIT_COST = item.UnitCost,
                UOM = item.UOM,
                VARIANCE = item.Variance,
                TRANSACTION_DATE = item.TransactionDate
            };
        }

        public async Task<EamisPropertyLedgerDTO> Insert(EamisPropertyLedgerDTO item)
        {
            try
            {
                EAMISPROPERTYLEDGER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<EamisPropertyLedgerDTO> Update(EamisPropertyLedgerDTO item)
        {
            try
            {
                EAMISPROPERTYLEDGER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<EamisPropertyLedgerDTO> Delete(EamisPropertyLedgerDTO item)
        {
            try
            {
                EAMISPROPERTYLEDGER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }
        private int GetTotalOnHand(string itemCode, DateTime asOfDate)
        {
            var itemId = _ctx.EAMIS_PROPERTYITEMS.Where(i => i.PROPERTY_NO == itemCode).AsNoTracking().Select(i => i.ID).FirstOrDefault();

            var result = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS
                             .Join(_ctx.EAMIS_DELIVERY_RECEIPT,
                             d => d.DELIVERY_RECEIPT_ID,
                             h => h.ID,
                             (d, h) => new { d, h })
                             .Where(d => d.h.DATE_RECEIVED <= asOfDate &&
                                         d.d.ITEM_ID == itemId)
                             .GroupBy(g => g.d.ITEM_ID)
                             .Select(v => v.Sum(t => t.d.QTY_RECEIVED))
                             .FirstOrDefault();
            return result;
        }
        private int GetTotalReceived(string itemCode, DateTime asOfDate)
        {
            var total = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                            d => d.PROPERTY_TRANS_ID,
                            h => h.ID,
                            (d, h) => new { d, h })
                            .Where(s => s.h.TRANSACTION_STATUS == DocStatus.Draft &&
                                        s.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
                                        s.d.ITEM_CODE == itemCode &&
                                        s.d.TIME_STAMP <= asOfDate)
                            .GroupBy(g => g.d.ITEM_CODE)
                            .Select(t => t.Sum(s => s.d.QTY))
                            .FirstOrDefault();
            return total;
        }

        private int GetTotalIssued(string itemCode, DateTime asOfDate)
        {
            var total = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                            d => d.PROPERTY_TRANS_ID,
                            h => h.ID,
                            (d, h) => new { d, h })
                            .Where(s => s.h.TRANSACTION_STATUS == DocStatus.Draft &&
                                        s.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance &&
                                        s.d.ITEM_CODE == itemCode &&
                                        s.d.TIME_STAMP <= asOfDate)
                            .GroupBy(g => g.d.ITEM_CODE)
                            .Select(t => t.Sum(s => s.d.QTY))
                            .FirstOrDefault();
            return total;
        }

        public async Task<DataList<EamisPropertyLedgerDTO>> List(EamisPropertyLedgerDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYLEDGER> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);

            return new DataList<EamisPropertyLedgerDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };

        }

        public async Task<DataList<EamisPropertyLedgerDTO>> ListForCreation(EamisPropertyLedgerDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYLEDGER> query = FilteredEntitiesForCreation(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);

            var result = new DataList<EamisPropertyLedgerDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };

            //Update values
            for (int intRow = 0; intRow < result.Items.Count(); intRow++)
            {
                result.Items[intRow].TotalReceived = GetTotalReceived(result.Items[intRow].ItemCode, result.Items[intRow].AsOfDate);
                result.Items[intRow].TotalIssued = GetTotalIssued(result.Items[intRow].ItemCode, result.Items[intRow].AsOfDate);
                result.Items[intRow].UOM = GetUnitOfMeasure(result.Items[intRow].ItemCode);
                result.Items[intRow].TotalOnHandDR = GetTotalOnHand(result.Items[intRow].ItemCode, result.Items[intRow].AsOfDate);
            }

            return result;
        }

        private IQueryable<EAMISPROPERTYLEDGER> FilteredEntities(EamisPropertyLedgerDTO filter, IQueryable<EAMISPROPERTYLEDGER> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYLEDGER>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            var query = custom_query ?? _ctx.EAMIS_PROPERTY_LEDGER;
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYLEDGER> FilteredEntitiesForCreation(EamisPropertyLedgerDTO filter, IQueryable<EAMISPROPERTYLEDGER> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYLEDGER>(true);

            //Get all received item
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                            d => d.PROPERTY_TRANS_ID,
                            h => h.ID,
                            (d, h) => new { d, h })
                            .Where(s => s.h.TRANSACTION_STATUS == DocStatus.Draft &&
                                        s.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance &&
                                        s.d.TIME_STAMP <= filter.AsOfDate)
                            .GroupBy(g => new {
                                g.d.ITEM_CODE,
                                g.d.TIME_STAMP,
                                g.d.ITEM_DESCRIPTION,
                                g.d.RESPONSIBILITY_CODE,
                                g.d.UNIT_COST
                            })
                            .Select(i => new EAMISPROPERTYLEDGER
                            {
                                ID = 0,
                                ITEM_CODE = i.Key.ITEM_CODE,
                                ITEM_DESC = i.Key.ITEM_DESCRIPTION,
                                PHYSICAL_COUNT = 0,//User input
                                REMARKS = string.Empty,//user input
                                RESPONSIBILITY_CODE = i.Key.RESPONSIBILITY_CODE,//item responsibility center
                                TOTAL_ISSUED = 0,
                                TOTAL_ON_HAND_DR = 0,
                                TOTAL_RECEIVED = 0,
                                TOTAL_VALUE_OH = 0,
                                TOTAL_VALUE_PC = 0,
                                TOTAL_VALUE_VAR = 0,
                                UNIT_COST = i.Key.UNIT_COST,
                                UOM = string.Empty,
                                VARIANCE = 0,
                                AS_OF_DATE = filter.AsOfDate,
                                TRANSACTION_DATE = i.Key.TIME_STAMP
                            });
            return query.Where(predicate);
        }
        private string GetUnitOfMeasure(string itemCode)
        {
            var unit = _ctx.EAMIS_PROPERTYITEMS
                           .Join(_ctx.EAMIS_UNITOFMEASURE,
                           p => p.UOM_ID,
                           m => m.ID,
                           (p, m) => new { p, m })
                           .Where(i => i.p.PROPERTY_NO == itemCode)
                           .Select(v => v.m.UOM_DESCRIPTION)
                           .FirstOrDefault();
            return unit;

        }

        private IQueryable<EAMISPROPERTYLEDGER> PagedQuery(IQueryable<EAMISPROPERTYLEDGER> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EamisPropertyLedgerDTO> QueryToDTO(IQueryable<EAMISPROPERTYLEDGER> query)
        {
            return query.Select(x => new EamisPropertyLedgerDTO
            {
                Id = x.ID,
                AsOfDate = x.AS_OF_DATE,
                ItemCode = x.ITEM_CODE,
                ItemDesc = x.ITEM_DESC,
                PhysicalCounnt = x.PHYSICAL_COUNT,
                Remarks = x.REMARKS,
                ResponsibilityCode = x.RESPONSIBILITY_CODE,
                TotalIssued = x.TOTAL_ISSUED,
                TotalOnHandDR = x.TOTAL_ON_HAND_DR,
                TotalReceived = x.TOTAL_RECEIVED,
                TotalValueOH = x.TOTAL_VALUE_OH,
                TotalValuePC = x.TOTAL_VALUE_PC,
                TotalValueVar = x.TOTAL_VALUE_VAR,
                UnitCost = x.UNIT_COST,
                UOM = x.UOM,
                Variance = x.VARIANCE,
                TransactionDate = x.TRANSACTION_DATE
            });
        }

    }
}
