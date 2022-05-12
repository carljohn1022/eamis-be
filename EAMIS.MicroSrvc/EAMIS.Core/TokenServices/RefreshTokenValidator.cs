using EAMIS.Core.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.TokenServices
{
    public class RefreshTokenValidator
    {
        private readonly AuthenticationConfiguration _config;

        public RefreshTokenValidator(AuthenticationConfiguration config)
        {
            _config = config;
        }

        public bool Validate(string RefreshToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshAccessTokenSecret)),
                ValidAudience = _config.Audience,
                ValidIssuer = _config.Issuer,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,

            };
            try
            {
                tokenHandler.ValidateToken(RefreshToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool VerifyToken(string RefreshToken)
        {
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshAccessTokenSecret)),
                ValidAudience = _config.Audience,
                ValidIssuer = _config.Issuer,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(RefreshToken, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception e)
            {
                e.ToString(); //something else happened
                throw;
            }
            //... manual validations return false if anything untoward is discovered
            return validatedToken != null;
        }
    }
}
