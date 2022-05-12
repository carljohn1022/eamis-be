using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISROLES
    {
        [Key]
        public int ID  { get; set; }
        public string ROLE_NAME { get; set; }
        public bool IS_DELETED { get; set; }
        public List<EAMISUSERROLES> USERROLES { get; set; }
      
    }
}
