using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
   public class EAMISRESPONSIBILITYCENTER
    {
        [Key]
        public int ID { get; set; }
        public string MAIN_GROUP_CODE { get; set; }
        public string MAIN_GROUP_DESC { get; set; }
        public string SUB_GROUP_CODE { get; set; }
        public string SUB_GROUP_DESC { get; set; }
        public string OFFICE_CODE { get; set; }
        public string OFFICE_DESC { get; set; }
        public string UNIT_CODE { get; set; }
        public string UNIT_DESC { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string RESPONSIBILITY_CENTER { get; set; }
        public string LOCATION_CODE { get; set; }
        public string LOCATION_DESC { get; set; }
    }
}
