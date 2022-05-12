using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISREGION
    {
        [Key]
        public int REGION_CODE { get; set; }
        public string PSGCODE { get; set; }
        public string REGION_DESCRIPTION { get; set; }
        [ForeignKey("PROVINCE_CODE")]
        public List<EAMISPROVINCE> PROVINCE { get; set; }
       
    }
}
