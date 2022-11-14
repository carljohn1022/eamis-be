using EAMIS.Common.DTO.Ais;
using EAMIS.Common.DTO.Approval;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Approval
{
    public class EamisApprovalSetupDetailsRepository : IEamisApprovalSetupDetailsRepository
    {
        private EAMISContext _ctx;
        private AISContext _aisctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisApprovalSetupDetailsRepository(EAMISContext ctx, AISContext aisctx)
        {
            _ctx = ctx;
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<DataList<EamisApprovalSetupDetailsDTO>> List(EamisApprovalSetupDetailsDTO filter, PageConfig config)
        {
            IQueryable<EAMISAPPROVALSETUPDETAILS> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            var result = new DataList<EamisApprovalSetupDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
            var personnelInfo = _aisctx.Personnel.AsNoTracking().ToList();

            foreach (var item in result.Items)
            {
                item.User.PersonnelInfo = new AisPersonnelDTO
                {
                    LastName = personnelInfo.FirstOrDefault(i => i.Id == item.User.PersonnelId).LastName,
                    FirstName = personnelInfo.FirstOrDefault(i => i.Id == item.User.PersonnelId).FirstName,
                    MiddleName = personnelInfo.FirstOrDefault(i => i.Id == item.User.PersonnelId).MiddleName
                };
            }
            return result;
        }

        private IQueryable<EamisApprovalSetupDetailsDTO> QueryToDTO(IQueryable<EAMISAPPROVALSETUPDETAILS> query)
        {
            return query.Select(x => new EamisApprovalSetupDetailsDTO
            {
                Id = x.ID,
                ApprovalSetupId = x.APPROVALSETUP_ID,
                ViewLevel = x.VIEW_LEVEL,
                SignatoryId = x.SIGNATORY_ID,
                MinAmount = x.MIN_AMOUNT,
                MaxAmount = x.MAX_AMOUNT,
                ApprovalSetup = _ctx.EAMIS_APPROVAL_SETUP.AsNoTracking().Select(s => new EamisApprovalSetupDTO
                {
                    Id = s.ID,
                    ModuleName = s.MODULE_NAME,
                    MaxLevel = s.MAX_LEVEL
                }).Where(i => i.Id == x.APPROVALSETUP_ID).FirstOrDefault(),
                User = _ctx.EAMIS_USERS.AsNoTracking().Select(u => new EamisUsersDTO
                {
                    User_Id = u.USER_ID,
                    Username = u.USERNAME,
                    PersonnelId = u.USER_INFO_ID
                }).Where(i => i.User_Id == x.SIGNATORY_ID).FirstOrDefault()
            });
        }

        private IQueryable<EAMISAPPROVALSETUPDETAILS> FilteredEntities(EamisApprovalSetupDetailsDTO filter, IQueryable<EAMISAPPROVALSETUPDETAILS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISAPPROVALSETUPDETAILS>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ApprovalSetupId != 0)
                predicate = predicate.And(x => x.APPROVALSETUP_ID == filter.ApprovalSetupId);
            if (filter.SignatoryId != 0)
                predicate = predicate.And(x => x.SIGNATORY_ID == filter.SignatoryId);
            if (filter.ViewLevel != 0)
                predicate = predicate.And(x => x.VIEW_LEVEL == filter.ViewLevel);
            if (filter.MinAmount != 0)
                predicate = predicate.And(x => x.MIN_AMOUNT == filter.MinAmount);
            if (filter.MaxAmount != 0)
                predicate = predicate.And(x => x.MAX_AMOUNT == filter.MaxAmount);
            var query = custom_query ?? _ctx.EAMIS_APPROVAL_SETUP_DETAILS;
            return query.Where(predicate);
        }

        public IQueryable<EAMISAPPROVALSETUPDETAILS> PagedQuery(IQueryable<EAMISAPPROVALSETUPDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisApprovalSetupDetailsDTO> Insert(EamisApprovalSetupDetailsDTO item)
        {
            try
            {
                EAMISAPPROVALSETUPDETAILS data = MapToEntity(item);
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

        private EAMISAPPROVALSETUPDETAILS MapToEntity(EamisApprovalSetupDetailsDTO item)
        {
            if (item == null) return new EAMISAPPROVALSETUPDETAILS();
            return new EAMISAPPROVALSETUPDETAILS
            {
                ID = item.Id,
                APPROVALSETUP_ID = item.ApprovalSetupId,
                SIGNATORY_ID = item.SignatoryId,
                VIEW_LEVEL = item.ViewLevel,
                MIN_AMOUNT = item.MinAmount,
                MAX_AMOUNT = item.MaxAmount
            };
        }

        public async Task<EamisApprovalSetupDetailsDTO> Update(EamisApprovalSetupDetailsDTO item)
        {
            try
            {
                EAMISAPPROVALSETUPDETAILS data = MapToEntity(item);
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

        public async Task<EamisApprovalSetupDetailsDTO> Delete(EamisApprovalSetupDetailsDTO item)
        {
            try
            {
                EAMISAPPROVALSETUPDETAILS data = MapToEntity(item);
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
