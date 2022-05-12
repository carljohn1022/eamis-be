using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROVINCE
    {
        [Key]
        public int PROVINCE_CODE { get; set; }
        public int REGION_CODE { get; set; }
        public string PSGCCODE { get; set; }
        public string PROVINCE_DESCRITION { get; set; }
      
        [ForeignKey("REGION_CODE")]
        public EAMISREGION REGION { get; set; }
      
    }
}
