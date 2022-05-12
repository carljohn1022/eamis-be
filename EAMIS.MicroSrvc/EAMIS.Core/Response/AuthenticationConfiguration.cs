using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Response
{
    public class AuthenticationConfiguration
    {
     
        public string AccessTokenSecret { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public string RefreshAccessTokenSecret { get; set; }
        public int RefreshAccessTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string DenrEamis { get; set; }
        public string DenrAis { get; set; }
    }
}
