using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using LinqKit;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EAMIS.Core.TokenServices;
using System.Net;
using EAMIS.Core.ContractRepository;
using System.Configuration;
using EAMIS.Core.Response.DTO;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Common.DTO.Branch_Maintenance;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisUserRolesRepository : IEamisUserRolesRepository
    {
        private readonly EAMISContext _ctx;

        public EamisUserRolesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<DataList<EamisUserRolesDTO>> List(EamisUserRolesDTO filter)
        {
            IQueryable<EAMISUSERROLES> query = FilteredEntities(filter);

            //string resolved_sort = config.SortBy ?? "ApplicationId";
            //bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;

            //int resolved_size = config.Size ?? _maxPageSize;
            //if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            //int resolved_index = config.Index ?? 1;

            //query = OrderEntities(query, resolved_sort, resolved_isAscending);
            var paged = PagedQuery(query);
            return new DataList<EamisUserRolesDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }
        private EamisUserRolesDTO MapToDTO(EAMISUSERROLES item)
        {
            if (item == null) return new EamisUserRolesDTO();
           // List<EamisUserRolesDTO> RolesList = new List<EamisUserRolesDTO>();
           
            return new EamisUserRolesDTO
            {
                Id = item.ID,
                UserId = item.USER_ID,
                RoleId = item.ROLE_ID,
                IsDeleted = item.IS_DELETED,
                
            };

        }
        private IQueryable<EamisUserRolesDTO> QueryToDTO(IQueryable<EAMISUSERROLES> query)
        {
            return query.Select(x => new EamisUserRolesDTO
            {
                Id = x.ID,
                UserId = x.USER_ID,
                RoleId = x.ROLE_ID,
                IsDeleted = x.IS_DELETED,
                Roles = new EamisRolesDTO
                {
                    Id = x.ROLES.ID,
                    Role_Name = x.ROLES.ROLE_NAME,
                    IsDeleted = x.ROLES.IS_DELETED,
                },
                Users = _ctx.EAMIS_USERS.AsNoTracking().Select(v => new EamisUsersDTO
                {
                    User_Id = v.USER_ID,
                    Username = v.USERNAME,
                    Branch = v.BRANCH,
                    UserInfoId = v.USER_INFO_ID,
                    Password_Hash = v.PASSWORD_HASH,
                    Password_Salt = v.PASSWORD_SALT,
                    IsActive = v.IS_ACTIVE,
                    IsDeleted = v.IS_DELETED,
                    IsBlocked = v.IS_BLOCKED,
                    AgencyEmployeeNumber = v.AGENCY_EMPLOYEE_NUMBER,
                    BranchGroup = _ctx.branch.AsNoTracking().Where(h => h.BranchID == v.BRANCH)
                                            .Select(h => new EamisBranchDTO
                                            {
                                                BranchID = h.BranchID,
                                                BranchDescription = h.BranchDescription
                                            }).FirstOrDefault(),

                }).Where(v => v.User_Id == x.USER_ID).ToList()
            });
        }
        private EAMISUSERROLES MapToEntity(EamisUserRolesDTO item)
        {
            if (item == null) return new EAMISUSERROLES();
            return new EAMISUSERROLES
            {
                ID = item.Id,
                USER_ID = item.UserId,
                ROLE_ID = item.RoleId,
                IS_DELETED = item.IsDeleted,
                
              
            };
        }
        private IQueryable<EAMISUSERROLES> FilteredEntities(EamisUserRolesDTO filter, IQueryable<EAMISUSERROLES> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISUSERROLES>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.UserId != null && filter.UserId != 0)
                predicate = predicate.And(x => x.USER_ID == filter.UserId);
            if (filter.RoleId != null && filter.RoleId != 0)
                predicate = predicate.And(x => x.ROLE_ID == filter.RoleId);
            if (filter.IsDeleted != null && filter.IsDeleted != false)
                predicate = predicate.And(x => x.IS_DELETED == filter.IsDeleted);
            var query = custom_query ?? _ctx.EAMIS_USER_ROLES;
            return query.Where(predicate);
        }
        public IQueryable<EAMISUSERROLES> PagedQuery(IQueryable<EAMISUSERROLES> query)
        {
            return query;
        }

        public async Task<EamisUserRolesDTO> Delete(EamisUserRolesDTO item)
        {

            EAMISUSERROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisUserRolesDTO> Insert(EamisUserRolesDTO item)
        {
            EAMISUSERROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisUserRolesDTO> Update(EamisUserRolesDTO item)
        {

            EAMISUSERROLES data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
