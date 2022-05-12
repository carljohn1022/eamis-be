using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISUSERLOGIN
    {
        [Key]
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public string COMPUTER_NAME { get; set; }
        public string LOGED_IN_TIMESTAMP { get; set; }
        public string LOGED_OUT_TIMESTAMP { get; set; }
        public bool IS_LOGOUT { get; set ;}
        public EAMISUSERS EAMIS_USERS { get; set; }
    }
}
