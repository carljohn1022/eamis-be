using EAMIS.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.TokenServices
{

    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _config;
        TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration config, TokenGenerator tokenGenerator)
        {
            _config = config;
            _tokenGenerator = tokenGenerator;
        }

      public string GenerateToken()
        {
            return _tokenGenerator.GenerateToken(_config.RefreshAccessTokenSecret,
                _config.Issuer,
                _config.Audience,
                _config.RefreshAccessTokenExpirationMinutes);
        }
    }
}
