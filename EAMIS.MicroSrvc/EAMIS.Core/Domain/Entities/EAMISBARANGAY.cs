using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISBARANGAY
    {
        public string BRGY_DESCRIPTION { get; set; }
        public int REGION_CODE { get; set; }
        public int PROVINCE_CODE { get; set; }
        public int MUNICIPALITY_CODE { get; set; }
        [Key]
        public int BRGY_CODE { get; set; }

        [ForeignKey("MUNICIPALITY_CODE")]
        public virtual EAMISMUNICIPALITY MUNICIPALITY { get; set; }
        [ForeignKey("PROVINCE_CODE")]
        public EAMISPROVINCE PROVINCE { get; set; }
        [ForeignKey("REGION_CODE")]
        public virtual EAMISREGION REGION { get; set; }
       

     
       

    }
}
