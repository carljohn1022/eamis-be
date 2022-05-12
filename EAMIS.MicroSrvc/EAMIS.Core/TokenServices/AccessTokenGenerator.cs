using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.TokenServices
{
    public class AccessTokenGenerator
    {
        private readonly AuthenticationConfiguration _config;
        TokenGenerator _tokenGenerator;
     //  private List<EAMISUSERROLES> listofUserRoles { get; set; }
        EAMISContext _ctx;
        public AccessTokenGenerator(AuthenticationConfiguration config,TokenGenerator tokenGenerator,EAMISContext ctx)
        {
            _config = config;
            _tokenGenerator = tokenGenerator;
            _ctx = ctx;
        }

        public async Task<string> GenerateToken(EAMISUSERS user)
        {
            SecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            
            var listofRoles = await _ctx.EAMIS_USER_ROLES.AsNoTracking().Where(x=>x.USER_ID == user.USER_ID).Select(item=> new EamisUserRolesDTO {
            Id = item.ID,
            UserId = item.USER_ID,
            RoleId = item.ROLE_ID,
            IsDeleted = item.IS_DELETED,
            Roles = new EamisRolesDTO
            {
                Id = item.ROLES.ID,
                Role_Name = item.ROLES.ROLE_NAME,
                IsDeleted = item.ROLES.IS_DELETED,

            }
            }).ToListAsync();

            var validClaims = new List<Claim>
           {
             new Claim("Id",user.USER_ID.ToString()),
             new Claim(ClaimTypes.Name,user.USERNAME),
             new Claim(ClaimTypes.Role,string.Join(",",listofRoles.Select(y=>y.Roles.Role_Name).ToArray())),
           };



            return _tokenGenerator.GenerateToken(_config.AccessTokenSecret,
                _config.Issuer,
                _config.Audience,
                _config.AccessTokenExpirationMinutes,
                validClaims);


        }
    }
}
