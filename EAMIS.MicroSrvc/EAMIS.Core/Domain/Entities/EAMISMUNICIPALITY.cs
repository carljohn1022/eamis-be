using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISMUNICIPALITY
    {

        [Key]
        public int MUNICIPALITY_CODE { get; set; }
        public string PSGCODE { get; set; }
        public string CITY_MUNICIPALITY_DESCRIPTION { get; set; }
        [ForeignKey("REGION_CODE")]
        public EAMISREGION REGION { get; set; }
        public int REGION_CODE { get; set; }
        [ForeignKey("PROVINCE_CODE")]
        public EAMISPROVINCE PROVINCE { get; set; }
        public int PROVINCE_CODE { get; set; }
        [ForeignKey("BARANGAY_CODE")]
        public virtual List<EAMISBARANGAY> BARANGAY { get; set; }
     

    }
}
