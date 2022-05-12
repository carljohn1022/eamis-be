using EAMIS.Common.DTO.Ais;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.TokenServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EAMIS.Common.DTO.Masterfiles.EamisUserloginDTO;

namespace EAMIS.Core.Response
{
    public class Authenticator
    {
        EAMISContext _ctx;
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenRepositories _refreshTokenRepositories;
        private readonly TokenGenerator _tokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly AISContext _aisctx;
        public Authenticator(EAMISContext ctx, AccessTokenGenerator accessTokenGenerator, TokenGenerator tokenGenerator, RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepositories refreshTokenRepositories, AISContext aisctx)
        {
            _ctx = ctx;
            _accessTokenGenerator = accessTokenGenerator;
            _tokenGenerator = tokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepositories = refreshTokenRepositories;
            _aisctx = aisctx;
        }

        public  async Task<LoginDTO> User(LoginDTO item)
        {
            var role = await _ctx.EAMIS_ROLES.AsNoTracking().ToListAsync();
            var user = await _ctx.EAMIS_USERS.AsNoTracking().Where(x=>x.IS_ACTIVE == true && x.USER_ID == item.UsersToken.User_Id).FirstOrDefaultAsync();
            var userLogin = await _ctx.EAMIS_USER_LOGIN.AsNoTracking().Where(x => x.USER_ID == user.USER_ID && x.IS_LOGOUT != true).OrderByDescending(x=>x.ID).FirstOrDefaultAsync();
            var codeList = await _aisctx.CodeListValue.AsNoTracking().Where(x => x.CodeListType == "Sex" && x.IsActive == true && x.IsDeleted == false).ToListAsync();
            var userRole = await _ctx.EAMIS_USER_ROLES.AsNoTracking().Where(x => x.USER_ID == user.USER_ID).ToListAsync();
            var currentLogedinUserInfo = await _aisctx.Personnel.AsNoTracking().SingleOrDefaultAsync(x => x.AgencyEmployeeNumber == user.AGENCY_EMPLOYEE_NUMBER);
           // await Task.WhenAll(user,codeList,currentLogedinUserInfo);
            return new LoginDTO
            {
                ComputerName = Dns.GetHostName(),
                UsersToken = new UserDTO
                {
                    Id = userLogin.ID,
                    User_Id = user.USER_ID,
                    Username = user.USERNAME,
                    AccessToken = await _accessTokenGenerator.GenerateToken(user),
                    RefreshToken = _refreshTokenGenerator.GenerateToken(),   
                    PersonnelInfo = new AisPersonnelDTO
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
                        CodeValue = codeList.Select(item => new AisCodeListValueDTO { Name = codeList.FirstOrDefault(x => x.Id == currentLogedinUserInfo.SexId).Name }).FirstOrDefault(),
                    },
                    userRole = userRole.Select(item => new EamisUserRolesDTO
                    {
                        RoleId = item.ROLE_ID,
                        UserId = item.USER_ID,
                        Roles = new EamisRolesDTO
                        {
                            Role_Name = role.FirstOrDefault(x => x.ID == item.ROLE_ID).ROLE_NAME,
                        },
                    }).ToList(),
                    
                },
               
            };
        }
    }
}
