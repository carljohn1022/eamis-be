using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{

    public class EamisRoleModuleLinkRepository : IEamisRoleModuleLinkRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisRoleModuleLinkRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisRoleModuleLinkDTO> Delete(EamisRoleModuleLinkDTO item)
        {
            try
            {
                EAMISROLEMODULELINK data = MapToEntity(item);
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
        public async Task<string> GetAgencyName(int userId)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_USERS.Where(s => s.USER_ID == userId).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].AGENCY_EMPLOYEE_NUMBER.ToString();
            }
            return retValue;
        }
        public async Task<EamisUserRolesDTO> GetUserIdList(int userId)
        {

            var result = await Task.Run(() => _ctx.EAMIS_USER_ROLES.AsNoTracking().FirstOrDefaultAsync(x => x.USER_ID == userId)).ConfigureAwait(false);
            return new EamisUserRolesDTO
            {
                Id = result.ID,
                UserId = result.USER_ID,
                RoleId = result.ROLE_ID,
                IsDeleted = result.IS_DELETED,
                ModulesRoles = _ctx.EAMIS_ROLE_MODULE_LINK.Select(r => new EamisRoleModuleLinkDTO
                {
                    Id = r.ID,
                    ModuleId = r.MODULE_ID,
                    RoleId = r.ROLE_ID,
                    ViewRight = r.VIEW_RIGHT,
                    UpdateRight = r.UPDATE_RIGHT,
                    DeactivateRight = r.DEACTIVATE_RIGHT,
                    PrintRight = r.PRINT_RIGHT,
                    IsActive = r.IS_ACTIVE,
                    InsertRight = r.INSERT_RIGHT,
                    UserId = r.USER_ID,
                    ModulesNameList = _ctx.EAMIS_MODULES.Select(v => new EamisModulesDTO
                    {
                        Id = v.ID,
                        ModuleName = v.MODULE_NAME,
                        IsActive = v.IS_ACTIVE
                    }).Where(i => i.Id == r.MODULE_ID).FirstOrDefault(),
                }).Where(i => i.UserId == result.USER_ID).ToList(),
            };
        }
        public Task<bool> Validate(int UserId)
        {
            return _ctx.EAMIS_ROLE_MODULE_LINK.AsNoTracking().AnyAsync(x => x.USER_ID == UserId);
        }
        public async Task<EamisRoleModuleLinkDTO> Insert(EamisRoleModuleLinkDTO item)
        {
            try
            {
                EAMISROLEMODULELINK data = MapToEntity(item);
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

        public async Task<DataList<EamisRoleModuleLinkDTO>> List(EamisRoleModuleLinkDTO filter, PageConfig config)
        {
            IQueryable<EAMISROLEMODULELINK> query = FilteredEntities(filter);

            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisRoleModuleLinkDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
        }

        public IQueryable<EAMISROLEMODULELINK> PagedQuery(IQueryable<EAMISROLEMODULELINK> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisRoleModuleLinkDTO> Update(EamisRoleModuleLinkDTO item)
        {
            try
            {
                EAMISROLEMODULELINK data = MapToEntity(item);
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
        private EamisRoleModuleLinkDTO MapToDTO(EAMISROLEMODULELINK item)
        {
            if (item == null) return new EamisRoleModuleLinkDTO();
            return new EamisRoleModuleLinkDTO
            {
                Id = item.ID,
                RoleId = item.ROLE_ID,
                ViewRight = item.VIEW_RIGHT,
                UpdateRight = item.UPDATE_RIGHT,
                DeactivateRight = item.DEACTIVATE_RIGHT,
                InsertRight = item.INSERT_RIGHT,
                ModuleId = item.MODULE_ID,
                PrintRight = item.PRINT_RIGHT,
                IsActive = item.IS_ACTIVE,
                UserId = item.USER_ID
            };

        }
        private EAMISROLEMODULELINK MapToEntity(EamisRoleModuleLinkDTO item)
        {
            if (item == null) return new EAMISROLEMODULELINK();
            return new EAMISROLEMODULELINK
            {
                ID = item.Id,
                ROLE_ID = item.RoleId,
                DEACTIVATE_RIGHT = item.DeactivateRight,
                INSERT_RIGHT = item.InsertRight,
                MODULE_ID = item.ModuleId,
                PRINT_RIGHT = item.PrintRight,
                UPDATE_RIGHT = item.UpdateRight,
                VIEW_RIGHT = item.ViewRight,
                IS_ACTIVE = item.IsActive,
                USER_ID = item.UserId
            };
        }
        private IQueryable<EamisRoleModuleLinkDTO> QueryToDTO(IQueryable<EAMISROLEMODULELINK> query)
        {
            return query.Select(x => new EamisRoleModuleLinkDTO
            {
                Id = x.ID,
                RoleId = x.ROLE_ID,
                ViewRight = x.VIEW_RIGHT,
                UpdateRight = x.UPDATE_RIGHT,
                DeactivateRight = x.DEACTIVATE_RIGHT,
                InsertRight = x.INSERT_RIGHT,
                ModuleId = x.MODULE_ID,
                PrintRight = x.PRINT_RIGHT,
                IsActive = x.IS_ACTIVE,
                UserId = x.USER_ID,
                Roles = _ctx.EAMIS_ROLES.Select(r => new EamisRolesDTO
                {
                    Id = r.ID,
                    Role_Name = r.ROLE_NAME,
                    IsDeleted = r.IS_DELETED
                }).Where(i => i.Id == x.ROLE_ID).FirstOrDefault()
            });
        }
        private IQueryable<EAMISROLEMODULELINK> FilteredEntities(EamisRoleModuleLinkDTO filter, IQueryable<EAMISROLEMODULELINK> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISROLEMODULELINK>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);
            var query = custom_query ?? _ctx.EAMIS_ROLE_MODULE_LINK;
            return query.Where(predicate);
        }

    }
}
