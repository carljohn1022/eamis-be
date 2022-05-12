using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.TokenServices
{
    public class TokenGenerator 
    {
        private readonly IConfiguration _config;
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        public TokenGenerator(IConfiguration config, AuthenticationConfiguration authenticationConfiguration)
        {

            _config = config;
            _authenticationConfiguration = authenticationConfiguration;
        }

      
       
        public string GenerateToken(string secretKey,string issuer,string audience,double expirationMinutes,IEnumerable<Claim> validClaims = null)
        {
            var _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(_authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                validClaims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_authenticationConfiguration.AccessTokenExpirationMinutes),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


       
    }
}
