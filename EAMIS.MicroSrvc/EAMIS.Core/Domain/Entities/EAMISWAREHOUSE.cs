using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISWAREHOUSE
    {
        [Key]
        public int ID { get; set; }
        public string WAREHOUSE_DESCRIPTION { get; set; }
        public string STREET_NAME { get; set; }
        public int REGION_CODE { get; set; }
        public int MUNICIPALITY_CODE { get; set; }
        public int PROVINCE_CODE { get; set; }
        public int BARANGAY_CODE { get; set; }
        public bool IS_ACTIVE { get; set; }
        [ForeignKey("BARANGAY_CODE")]
        public EAMISBARANGAY BARANGAY { get; set; }
        public List<EAMISPROPERTYITEMS> PROPERTY_ITEMS { get; set; }

    }
}
