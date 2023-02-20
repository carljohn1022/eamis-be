using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EAMIS.Core.TokenServices;
using System.Net;
using EAMIS.Core.ContractRepository;
using System.Configuration;
using EAMIS.Core.Response.DTO;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Common.DTO.Ais;
using EAMIS.Core.Domain.Entities.AIS;
using LinqKit;
using System.Linq;

namespace EAMIS.Core.BusinessLogic.Masterfiles
{
    public class EamisUsersRepository : IEamisUsersRepository
    {
        private readonly EAMISContext _ctx;
        private readonly AISContext _aisctx;

        private readonly int _maxPageSize;


        public EamisUsersRepository(EAMISContext ctx, AISContext aisctx)
        {
            _ctx = ctx;
            _aisctx = aisctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                            : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());

        }
        private EAMISUSERS MapToEntity(EamisUsersDTO item)
        {
            if (item == null) return new EAMISUSERS();
            return new EAMISUSERS
            {
                USER_ID = item.User_Id,
                USERNAME = item.Username,
                BRANCH = item.Branch,
                PASSWORD_HASH = item.Password_Hash,
                PASSWORD_SALT = item.Password_Salt,
                USER_INFO_ID = item.UserInfoId,
                IS_ACTIVE = item.IsActive,
                IS_DELETED = item.IsDeleted,
                AGENCY_EMPLOYEE_NUMBER = item.AgencyEmployeeNumber,
                IS_BLOCKED = item.IsBlocked,
            };
        }

        public async Task<EAMISUSERS> Register(RegisterDTO item) 
        {
            var aisPersonnelEmployeeId = await _aisctx.Personnel.AsNoTracking().Where(x => x.AgencyEmployeeNumber == item.AgencyEmployeeNumber).FirstOrDefaultAsync();
            if (aisPersonnelEmployeeId == null)
                return null;

                using var hmac = new HMACSHA256();

            var user = new EAMISUSERS
            {
                USER_ID = 0,
               // USER_INFO_ID = aisPersonnelEmployeeId.Id,
                AGENCY_EMPLOYEE_NUMBER = aisPersonnelEmployeeId.AgencyEmployeeNumber,
                USERNAME = item.Username,
                BRANCH = item.Branch,
                IS_DELETED = false,
                IS_ACTIVE = true,
                IS_BLOCKED = false,
                USER_INFO_ID = item.UserInfoId,
                PASSWORD_HASH = hmac.ComputeHash(Encoding.UTF8.GetBytes(item.Password)),
                PASSWORD_SALT = hmac.Key,

                };
                _ctx.Entry(user).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                return user;
        }
        public async Task<bool> UserExists(string Username,string AgencyEmployeeId)
        {
            return await _ctx.EAMIS_USERS.AnyAsync(x => x.USERNAME == Username.ToUpper() && x.AGENCY_EMPLOYEE_NUMBER == AgencyEmployeeId);
        }
      


        //public async Task<List<EamisUsersDTO>> listofUsers(EamisUsersDTO usersDTO)
        //{
        //    var query = _ctx.EAMIS_USERS.AsQueryable();
        //    query = query.Where(x => x.USERNAME == usersDTO.Username);
        //    query = query.Where(x => x.PASSWORD_HASH == usersDTO.Password_Hash);
        //    return await _ctx.EAMIS_USERS.AsNoTracking().Select(x => new EamisUsersDTO
        //    {
        //        User_Id = x.USER_ID,
        //        Username = x.USERNAME,
        //        Password_Hash = x.PASSWORD_HASH,
        //        Password_Salt = x.PASSWORD_SALT
        //    }).ToListAsync();

        //    // a class describes the contents of the objects that belong to it: it describes an aggregate of data fields(called instance variables), and defines the operations(called methods).
        //}
        
        private IQueryable<EAMISUSERS> FilteredEntities(EamisUsersDTO filter,IQueryable<EAMISUSERS> custom_query = null,bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISUSERS>(true);
            if (!string.IsNullOrEmpty(filter.Username)) predicate = (strict)
                     ? predicate.And(x => x.USERNAME.ToLower() == filter.Username.ToLower())
                     : predicate.And(x => x.USERNAME.ToLower().Contains(filter.Username.ToLower()));
            if (filter.IsActive) predicate = (strict)
                     ? predicate.And(x => x.IS_ACTIVE == filter.IsActive)
                     : predicate.And(x => x.IS_ACTIVE == false);
            if (filter.IsDeleted) predicate = (strict)
                    ? predicate.And(x => x.IS_DELETED == filter.IsActive)
                    : predicate.And(x => x.IS_DELETED == false);
            if (filter.IsBlocked) predicate = (strict)
                  ? predicate.And(x => x.IS_BLOCKED == filter.IsBlocked)
                  : predicate.And(x => x.IS_BLOCKED == false);
            var query = custom_query ?? _ctx.EAMIS_USERS;
        
            return query.Where(predicate);
        }

        private IQueryable<EamisUsersDTO> QueryToDTO(IQueryable<EAMISUSERS> query)
        {

            return query.Select(x => new EamisUsersDTO
            {
                User_Id = x.USER_ID,
                Username = x.USERNAME,
                Branch = x.BRANCH,
                UserInfoId = x.USER_INFO_ID,
                Password_Hash = x.PASSWORD_HASH,
                Password_Salt = x.PASSWORD_SALT,
                IsActive = x.IS_ACTIVE,
                IsDeleted = x.IS_DELETED,
                IsBlocked = x.IS_BLOCKED,
                AgencyEmployeeNumber = x.AGENCY_EMPLOYEE_NUMBER,
                UserRoles = x.USER_ROLES.Select(y => new EamisUserRolesDTO { Id = y.ID, RoleId = y.ROLE_ID, Roles = new EamisRolesDTO { Role_Name = y.ROLES.ROLE_NAME } }).ToList(),
                
                PersonnelInfo  = new AisPersonnelDTO
                {
                    //AgencyEmployeeNumber = x.PERSONNEL.AgencyEmployeeNumber,

                }

            });
        }
        public EamisUsersDTO MapToDTO(EAMISUSERS item)
        {

            if (item == null) return new EamisUsersDTO();
            var personnelInfo = _aisctx.Personnel.AsNoTracking().ToList();
            var codeList = _aisctx.CodeListValue.AsNoTracking().Where(x => x.CodeListType == "Sex" && x.IsActive == true && x.IsDeleted == false).ToList();

           // var zz = Task.WhenAll(personnelInfo, codeList);

           

            return new EamisUsersDTO
            {
                User_Id = item.USER_ID,
                Username = item.USERNAME,
                UserInfoId = item.USER_INFO_ID,
                Password_Hash = item.PASSWORD_HASH,
                Password_Salt = item.PASSWORD_SALT,
                IsActive = item.IS_ACTIVE,
                IsDeleted = item.IS_DELETED,
                IsBlocked = item.IS_BLOCKED,
                Branch = item.BRANCH,
                AgencyEmployeeNumber = item.AGENCY_EMPLOYEE_NUMBER,
                PersonnelInfo = new AisPersonnelDTO
                {
                    AgencyEmployeeNumber = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).AgencyEmployeeNumber,
                    FirstName = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).FirstName,
                    LastName = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).LastName,
                    MiddleName = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).MiddleName,
                    ExtensionName = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).ExtensionName,
                    SexId = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).SexId,
                    OfficeId = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).OfficeId,
                    //NickName = positionDetails.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).Position ?? null,

                    CodeValue = new AisCodeListValueDTO
                    {
                        Id = personnelInfo.FirstOrDefault(x => x.AgencyEmployeeNumber == item.AGENCY_EMPLOYEE_NUMBER).SexId,

                    },


                }
            };
        }
        public async Task<DataList<EamisUsersDTO>> PublicSearch(string searchType,string searchKey,PageConfig config)
        {
            List<EAMISUSERS> eamisUsers = new List<EAMISUSERS>();
            //if (!string.IsNullOrEmpty(searchKey))
            //{
            //    if(searchType == "IS BLOCKED")
            //        eamisUsers = await _ctx.EAMIS_USERS.AsNoTracking().Where(x=>x.IS_BLOCKED == true).ToListAsync();
            //    if (searchType == "IS ACTIVE")
            //        eamisUsers = await _ctx.EAMIS_USERS.AsNoTracking().Where(x => x.IS_ACTIVE == true).ToListAsync();
            //    if (searchType == "IS DELETED")
            //        eamisUsers = await _ctx.EAMIS_USERS.AsNoTracking().Where(x => x.IS_DELETED == true).ToListAsync();
            //}
            if(string.IsNullOrEmpty(searchType))
               eamisUsers = await _ctx.EAMIS_USERS.AsNoTracking().ToListAsync();


            string resolved_sort = config.SortBy ?? "Id";
            bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQueryMap(eamisUsers, resolved_size, resolved_index);
            return new DataList<EamisUsersDTO>
            {
                Count = eamisUsers.Count(),
                Items = paged.Select(x => MapToDTO(x)).ToList(),
            };
        }
        //public IQueryable<EAMISUSERS> IncludeEntities(IQueryable<EAMISUSERS> query)
        //{
        //    return query.Include(x => x.PERSONNEL_INFO);
        //}
        public async Task<DataList<EamisUsersDTO>> List(EamisUsersDTO filter, PageConfig config)
        {


            //string resolved_sort = config.SortBy ?? "ApplicationId";
            //bool resolved_isAscending = (config.IsAscending) ? config.IsAscending : false;

            //int resolved_size = config.Size ?? _maxPageSize;
            //if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            //int resolved_index = config.Index ?? 1;

            //query = OrderEntities(query, resolved_sort, resolved_isAscending);

            IQueryable<EAMISUSERS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            var result = new DataList<EamisUsersDTO>
            {
                Count = await QueryToDTO(paged).CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),

            };
            var personnelInfo = _aisctx.Personnel.AsNoTracking().ToList();

            foreach (var item in result.Items)
            {
                item.PersonnelInfo = new AisPersonnelDTO
                {
                    LastName = personnelInfo.FirstOrDefault(i => i.Id == item.UserInfoId).LastName,
                    FirstName = personnelInfo.FirstOrDefault(i => i.Id == item.UserInfoId).FirstName,
                    MiddleName = personnelInfo.FirstOrDefault(i => i.Id == item.UserInfoId).MiddleName
                };
            }
            return result;

        }

        public IQueryable<EAMISUSERS> PagedQuery(IQueryable<EAMISUSERS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public List<EAMISUSERS> PagedQueryMap(List<EAMISUSERS> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size).ToList();
        }

        public async Task<EAMISUSERS> GetUserName(string Username)
        {
            var getUserName = await _ctx.EAMIS_USERS.AsNoTracking().Where(x => x.USERNAME == Username).FirstOrDefaultAsync();
            return getUserName;
        }

        public async Task<EamisUsersDTO> ChangePassword(EamisUsersDTO item, string newPassword)
        {
            try
            {
                using var hmac = new HMACSHA256();

                item.Password_Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                item.Password_Salt = hmac.Key;
                //PASSWORD_HASH = hmac.ComputeHash(Encoding.UTF8.GetBytes(item.Password)),
                //PASSWORD_SALT = hmac.Key,

                EAMISUSERS data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return item;
        }

        public Task<bool> Validate(string EmployeeAgencyNumber)
        {
            return _ctx.EAMIS_USERS.AsNoTracking().AnyAsync(x => x.AGENCY_EMPLOYEE_NUMBER == EmployeeAgencyNumber);
        }
        public async Task<bool> UserExists(string Username)
        {
            return await _ctx.EAMIS_USERS.AnyAsync(x => x.USERNAME == Username);
        }
        public Task<bool> ValidateExistAgency(string EmployeeAgencyNumber)
        {
            return _ctx.EAMIS_USERS.AsNoTracking().AnyAsync(x => x.AGENCY_EMPLOYEE_NUMBER == EmployeeAgencyNumber);
        }
        public async Task<string> GetId(string AgencyEmployeeNumber)
        {
            string retValue = "";
            var result = await Task.Run(() => _aisctx.Personnel.Where(s => s.AgencyEmployeeNumber == AgencyEmployeeNumber).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].Id.ToString(); ;
            }
            return retValue;

        }
        public async Task<string> GetAgencyName(string AgencyEmployeeNumber)
        {
            string retValue = "";
            var result = await Task.Run(() => _aisctx.Personnel.Where(s => s.AgencyEmployeeNumber == AgencyEmployeeNumber).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].FirstName + " " + result[0].MiddleName + " " + result[0].LastName;
            }
            return retValue;
        }
        public async Task<string> GetOfficeName(int officeId)
        {
            string retValue = "";
            var result = await Task.Run(() => _aisctx.Office.Where(o => o.Id == officeId).AsNoTracking().ToList()).ConfigureAwait(false);
            if(result != null)
            {
                retValue = result[0].LongName;
            }
            return retValue;
        }
    }
}
