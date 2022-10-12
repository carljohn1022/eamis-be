using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYLEDGER
    {
        public int ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public string UOM { get; set; }
        public string RESPONSIBILITY_CODE { get; set; }
        public decimal UNIT_COST { get; set; }
        public int TOTAL_RECEIVED { get; set; }
        public int TOTAL_ISSUED { get; set; }
        public int TOTAL_ON_HAND_DR { get; set; }
        public decimal TOTAL_VALUE_OH { get; set; }
        public int PHYSICAL_COUNT { get; set; }
        public decimal TOTAL_VALUE_PC { get; set; }
        public int VARIANCE { get; set; }
        public decimal TOTAL_VALUE_VAR { get; set; }
        public string REMARKS { get; set; }
        public DateTime AS_OF_DATE { get; set; }
    }
}
