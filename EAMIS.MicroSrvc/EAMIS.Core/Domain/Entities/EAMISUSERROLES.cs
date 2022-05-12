using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISUSERROLES
    {
        [Key]
        public int ID { get; set; }
        public bool IS_DELETED { get; set; }
        public int ROLE_ID { get; set; }
        public int USER_ID { get; set; }
        public EAMISUSERS USERS  {get; set;}
        public EAMISROLES ROLES { get; set; }




    }
}
