using EAMIS.Core.Domain.Entities.AIS;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISUSERS
    {
        [Key]
        public int USER_ID { get; set; }
        public int USER_INFO_ID { get; set; }
        public string USERNAME { get; set; }
        public string? AGENCY_EMPLOYEE_NUMBER { get; set; }
        public byte[] PASSWORD_HASH { get; set; }
        public byte[] PASSWORD_SALT { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_DELETED { get; set; }
        public bool IS_BLOCKED { get; set; }
      
        public List<EAMISUSERROLES> USER_ROLES { get; set; }
        public List<EAMISUSERLOGIN> EAMISUSER_LOGIN { get; set; }
   
    }
}
