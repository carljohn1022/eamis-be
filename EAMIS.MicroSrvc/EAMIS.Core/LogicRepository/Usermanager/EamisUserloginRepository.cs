using EAMIS.Common.DTO.Ais;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report_Catalog;
using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.TokenServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static EAMIS.Common.DTO.Masterfiles.EamisUserloginDTO;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisUserloginRepository : IEamisUserloginRepository
    {
        IPAddress[] ipaddressv4 { get; set; }
        DateTime startDateTime = DateTime.UtcNow;
        TimeZoneInfo systemDateTimeZone = TimeZoneInfo.Local;
        private readonly EAMISContext _ctx;
        private readonly AISContext _aisctx;
        private readonly AccessTokenGenerator _AccesstokenService;
        private readonly RefreshTokenGenerator _refreshtokenService;
        private readonly IRefreshTokenRepositories _refreshTokenRepositories;
        private int counterTry = 0;
        private int counterTotal = 3;
        public LoginDTO UserId { get; set; }

        public EamisUserloginRepository(EAMISContext ctx,AISContext aisctx, AccessTokenGenerator AccesstokenService,RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepositories refreshTokenRepositories)
        {
            _ctx = ctx;
            _aisctx = aisctx;
            _AccesstokenService = AccesstokenService;
            _refreshtokenService = refreshTokenGenerator;
            _refreshTokenRepositories = refreshTokenRepositories;
        }

        public async Task<LoginDTO> GetById(LoginDTO item)
        {
            //var user = await _ctx.EAMIS_USERS.Include(x=>x.EAMISUSER_LOGIN).AsNoTracking().Where(x => x.IS_ACTIVE == true).Select(x => new EamisUsersDTO
            //{
            //    User_Id = x.USER_ID,
            //    Username = x.USERNAME,
            //    Password_Hash = x.PASSWORD_HASH,
            //    Password_Salt = x.PASSWORD_SALT,
            //    IsActive = x.IS_ACTIVE,
            //}).ToListAsync();

            var user1 = await _ctx.EAMIS_USER_LOGIN.AsNoTracking().Where(x => x.IS_LOGOUT == false && x.USER_ID == item.UsersToken.User_Id).Select(c => new LoginDTO
            {
                ComputerName = c.COMPUTER_NAME,
                UsersToken= new UserDTO
                {
                    Id = item.UsersToken.Id,
                    User_Id = item.UsersToken.User_Id,
                    RefreshToken = _refreshTokenRepositories.refreshToken.RefreshToken,
                }, 
            }).FirstOrDefaultAsync();
            return user1;
        }
        public async Task<bool> UsernameExist(string Username)
        {
            return await _ctx.EAMIS_USERS.AnyAsync(x => x.USERNAME == Username.ToUpper());
        }
        public async Task<LoginDTO> Login(UserLoginDTO item)
        {

            var role = await _ctx.EAMIS_ROLES.AsNoTracking().ToListAsync();
            var reportCatalog = await _ctx.EAMIS_REPORT_CATALOG.AsNoTracking().ToListAsync();
            var Modules = await _ctx.EAMIS_ROLE_MODULE_LINK.AsNoTracking().ToListAsync();
            var user = await _ctx.EAMIS_USERS.AsNoTracking().SingleOrDefaultAsync(x => x.USERNAME == item.Username && x.IS_DELETED == false && x.IS_ACTIVE == true && x.IS_BLOCKED == false);
            if (user == null) return null;
            var roleModuleManager = await _ctx.EAMIS_ROLE_MODULE_LINK.AsNoTracking().Where(x => x.USER_ID == user.USER_ID).ToListAsync();
            var Rolemanager = await _ctx.EAMIS_USER_ROLES.AsNoTracking().Where(x => x.USER_ID == user.USER_ID).ToListAsync();
            var roleReportLink = await _ctx.EAMIS_USER_REPORT_LINK.AsNoTracking().Where(x => x.USER_ID == user.USER_ID).ToListAsync();
            var codeList = await _aisctx.CodeListValue.AsNoTracking().Where(x => x.CodeListType == "Sex" && x.IsActive == true && x.IsDeleted == false).ToListAsync();
            var userRole = await _ctx.EAMIS_USER_ROLES.AsNoTracking().Where(x => x.USER_ID == user.USER_ID).ToListAsync();
            //var currentLogin = await _ctx.EAMIS_USER_LOGIN.SingleOrDefaultAsync(x => x.USER_ID == user.USER_ID && x.IS_LOGOUT == false);
            var currentLogedinUserInfo = await _aisctx.Personnel.AsNoTracking().SingleOrDefaultAsync(x => x.AgencyEmployeeNumber == user.AGENCY_EMPLOYEE_NUMBER);
            var getcurrentPosition = await _aisctx.AISPOSITION_VIEW.AsNoTracking().SingleOrDefaultAsync(x => x.AgencyEmployeeNumber == currentLogedinUserInfo.AgencyEmployeeNumber);

            if (currentLogedinUserInfo == null) return null;
            string hostName = Dns.GetHostName();
            //string ipv = Dns.GetHostByName(hostName).AddressList[2].ToString();
            using var hmac = new HMACSHA256(user.PASSWORD_SALT);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(item.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PASSWORD_HASH[i])
                {
                    return null;
                }
            }
            var input = new EAMISUSERLOGIN
            {
                ID = 0,
                USER_ID = user.USER_ID,
                COMPUTER_NAME = Dns.GetHostName().ToString(),
                LOGED_IN_TIMESTAMP = TimeZoneInfo.ConvertTimeFromUtc(startDateTime, systemDateTimeZone).ToString().ToUpper(),
                LOGED_OUT_TIMESTAMP = null,
                IS_LOGOUT = false,
            };
            _ctx.Entry(input).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            string aToken = await _AccesstokenService.GenerateToken(user);
            string rToken = _refreshtokenService.GenerateToken();
            RefreshTokenDTO refreshTokenDTO = new RefreshTokenDTO()
            {
                RefreshToken = rToken,
                UserId = input.USER_ID
            };
            await _refreshTokenRepositories.Create(refreshTokenDTO);

            return new LoginDTO
            {
                ComputerName = Dns.GetHostName().ToString(),
                UsersToken = new UserDTO
                {
                    Id = input.ID,
                    User_Id = user.USER_ID,
                    Username = user.USERNAME,
                    IsActive = user.IS_ACTIVE,
                    isBlocked = user.IS_BLOCKED,
                    isDeleted = user.IS_DELETED,
                    Branch = user.BRANCH,
                    AccessToken = aToken,
                    RefreshToken = rToken,
                    PersonnelInfo = new Common.DTO.Ais.AisPersonnelDTO
                    {
                        AgencyEmployeeNumber = currentLogedinUserInfo.AgencyEmployeeNumber,
                        LastName = currentLogedinUserInfo.LastName,
                        FirstName = currentLogedinUserInfo.FirstName,
                        MiddleName = currentLogedinUserInfo.MiddleName,
                        ExtensionName = currentLogedinUserInfo.ExtensionName,
                        NickName = currentLogedinUserInfo.NickName,
                        ProfilePhoto = currentLogedinUserInfo.ProfilePhoto,
                        OfficeId = currentLogedinUserInfo.OfficeId,
                        SexId = currentLogedinUserInfo.SexId,
                        CurrentPosition = getcurrentPosition.Position,
                        CodeValue = codeList.Select(item => new AisCodeListValueDTO { Name = codeList.FirstOrDefault(x => x.Id == currentLogedinUserInfo.SexId).Name }).FirstOrDefault(),
                    },
                    userRole = userRole.Select(item => new EamisUserRolesDTO
                    {
                        RoleId = item.ROLE_ID,
                        UserId = item.USER_ID,
                        Roles = new EamisRolesDTO
                        {
                            Id = role.FirstOrDefault(x => x.ID == item.ROLE_ID).ID,
                            Role_Name = role.FirstOrDefault(x => x.ID == item.ROLE_ID).ROLE_NAME,
                        },
                    }).ToList(),
                    RoleReportLink = roleReportLink.Select(v => new EamisUserReportLinkDTO
                    {
                        Id = v.ID,
                        UserId = v.USER_ID,
                        ReportId = v.REPORT_ID,
                        CanView = v.CAN_VIEW,
                        ReportCatalog = new EamisReportCatalogDTO
                        {
                            Id = reportCatalog.FirstOrDefault(x => x.ID == v.REPORT_ID).ID,
                            ReportDescription = reportCatalog.FirstOrDefault(x => x.ID == v.REPORT_ID).REPORT_DESCRIPTION,
                            ReportName = reportCatalog.FirstOrDefault(x => x.ID == v.REPORT_ID).REPORT_NAME,
                            Active = reportCatalog.FirstOrDefault(x => x.ID == v.REPORT_ID).ACTIVE = v.CAN_VIEW,
                        },
                    }).ToList(),
                    Modules = roleModuleManager.Select(r => new EamisRoleModuleLinkDTO
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
                            IsActive = v.IS_ACTIVE,
                            ViewRight = v.VIEW_RIGHT,
                            UpdateRight = v.UPDATE_RIGHT,
                            DeactivateRight = v.DEACTIVATE_RIGHT,
                            PrintRight = v.PRINT_RIGHT,
                            Active = v.ACTIVE,
                            InsertRight = v.INSERT_RIGHT,
                        }).Where(i => i.Id == r.MODULE_ID).FirstOrDefault(),
                    }).ToList(),
                },
            };
        }
        public async Task<UserLoginDTO> DirectBlockedUser(UserLoginDTO item)
        {
            var user = await _ctx.EAMIS_USERS.AsNoTracking().SingleOrDefaultAsync(x => x.USERNAME == item.Username);
            if (user == null)
            {
                return null;
            }

            var thisitem = new EAMISUSERS
            {
                USER_ID = user.USER_ID,
                USERNAME = user.USERNAME,
                AGENCY_EMPLOYEE_NUMBER = user.AGENCY_EMPLOYEE_NUMBER,
                USER_INFO_ID = user.USER_INFO_ID,
                IS_ACTIVE = true,
                IS_DELETED = false,
                IS_BLOCKED = true,
                BRANCH = user.BRANCH,
                PASSWORD_HASH = user.PASSWORD_HASH,
                PASSWORD_SALT = user.PASSWORD_SALT,
            };
            _ctx.Entry(thisitem).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
      
        public async Task<UserDTO> Logout(int Id, UserDTO item)
        {
            var user = await _ctx.EAMIS_USER_LOGIN.AsNoTracking().SingleOrDefaultAsync(x => x.ID == Id && x.IS_LOGOUT == false);
            if (user == null) return null;
            var input = new EAMISUSERLOGIN
            {
                ID = item.Id,
                USER_ID = item.User_Id,
                COMPUTER_NAME = user.COMPUTER_NAME,
                LOGED_IN_TIMESTAMP = user.LOGED_IN_TIMESTAMP,
                IS_LOGOUT = true,
                LOGED_OUT_TIMESTAMP = TimeZoneInfo.ConvertTimeFromUtc(startDateTime,systemDateTimeZone).ToString().ToUpper(),
            };
            _ctx.Entry(input).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;

        }
        public async Task<bool> UserLoginExists(string Username)
        {
            return await _ctx.EAMIS_USER_LOGIN.AsNoTracking().AnyAsync(x=> x.EAMIS_USERS.USERNAME == Username && x.IS_LOGOUT == false);
        }
    }
}
