using EAMIS.Common.DTO.Approval;
using EAMIS.Core.ContractRepository.Approval;
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

namespace EAMIS.Core.LogicRepository.Approval
{
    public class EamisForApprovalRepository : IEamisForApprovalRepository
    {
        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisForApprovalRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<EamisForApprovalDTO>> List(EamisForApprovalDTO filter, PageConfig config)
        {
            IQueryable<EAMISFORAPPROVAL> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisForApprovalDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisForApprovalDTO> QueryToDTO(IQueryable<EAMISFORAPPROVAL> query)
        {
            return query.Select(x => new EamisForApprovalDTO
            {
                Id = x.ID,
                TransactionType = x.TRANSACTION_TYPE,
                DocStatus = x.DOCSTATUS,
                TransactionNumber = x.TRANSACTION_NUMBER,
                TimeStamp = x.TIMESTAMP,
                Approver1Id = x.APPROVER1_ID,
                Approver1Status = x.APPROVER1_STATUS,
                Approver1Trandate = x.APPROVER1_TRANDATE,
                Approver1RejectedReason = x.APPROVER1_REJECTEDREASON,
                Approver2Id = x.APPROVER2_ID,
                Approver2Status = x.APPROVER2_STATUS,
                Approver2Trandate = x.APPROVER2_TRANDATE,
                Approver2RejectedReason = x.APPROVER2_REJECTEDREASON,
                Approver3Id = x.APPROVER3_ID,
                Approver3Status = x.APPROVER3_STATUS,
                Approver3Trandate = x.APPROVER3_TRANDATE,
                Approver3RejectedReason = x.APPROVER3_REJECTEDREASON
            });
        }

        private IQueryable<EAMISFORAPPROVAL> FilteredEntities(EamisForApprovalDTO filter, IQueryable<EAMISFORAPPROVAL> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISFORAPPROVAL>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.TransactionType != null && !string.IsNullOrEmpty(filter.TransactionType))
                predicate = predicate.And(x => x.TRANSACTION_TYPE == filter.TransactionType);
            if (filter.TransactionNumber != null && !string.IsNullOrEmpty(filter.TransactionNumber))
                predicate = predicate.And(x => x.TRANSACTION_NUMBER == filter.TransactionNumber);
            if (filter.Approver1Id != 0)
                predicate = predicate.And(x => x.APPROVER1_ID == filter.Approver1Id);
            if (filter.Approver1Status != null && !string.IsNullOrEmpty(filter.Approver1Status))
                predicate = predicate.And(x => x.APPROVER1_STATUS == filter.Approver1Status);

            if (filter.Approver2Id != 0)
                predicate = predicate.And(x => x.APPROVER2_ID == filter.Approver2Id);
            if (filter.Approver2Status != null && !string.IsNullOrEmpty(filter.Approver2Status))
                predicate = predicate.And(x => x.APPROVER2_STATUS == filter.Approver2Status);

            if (filter.Approver3Id != 0)
                predicate = predicate.And(x => x.APPROVER3_ID == filter.Approver3Id);
            if (filter.Approver3Status != null && !string.IsNullOrEmpty(filter.Approver3Status))
                predicate = predicate.And(x => x.APPROVER3_STATUS == filter.Approver3Status);

            var query = custom_query ?? _ctx.EAMIS_FOR_APPROVAL;
            return query.Where(predicate);
        }

        public IQueryable<EAMISFORAPPROVAL> PagedQuery(IQueryable<EAMISFORAPPROVAL> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisForApprovalDTO> Insert(EamisForApprovalDTO item)
        {
            try
            {
                EAMISFORAPPROVAL data = MapToEntity(item);
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

        private EAMISFORAPPROVAL MapToEntity(EamisForApprovalDTO item)
        {
            if (item == null) return new EAMISFORAPPROVAL();
            return new EAMISFORAPPROVAL
            {
                ID = item.Id,
                TRANSACTION_TYPE = item.TransactionType,
                TRANSACTION_NUMBER = item.TransactionNumber,
                TIMESTAMP = item.TimeStamp,
                DOCSTATUS = item.DocStatus,
                APPROVER1_ID = item.Approver1Id,
                APPROVER1_STATUS = item.Approver1Status,
                APPROVER1_TRANDATE = item.Approver1Trandate,
                APPROVER1_REJECTEDREASON = item.Approver1RejectedReason,
                APPROVER2_ID = item.Approver2Id,
                APPROVER2_STATUS = item.Approver2Status,
                APPROVER2_TRANDATE = item.Approver2Trandate,
                APPROVER2_REJECTEDREASON = item.Approver2RejectedReason,
                APPROVER3_ID = item.Approver3Id,
                APPROVER3_STATUS = item.Approver3Status,
                APPROVER3_TRANDATE = item.Approver3Trandate,
                APPROVER3_REJECTEDREASON = item.Approver3RejectedReason
            };
        }

        public async Task<EamisForApprovalDTO> Update(EamisForApprovalDTO item)
        {
            try
            {
                EAMISFORAPPROVAL data = MapToEntity(item);
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

        public async Task<EamisForApprovalDTO> Delete(EamisForApprovalDTO item)
        {
            try
            {
                EAMISFORAPPROVAL data = MapToEntity(item);
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
    }
}
