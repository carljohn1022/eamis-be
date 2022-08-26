using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYDEPRECIATION
    {
        public int ID { get; set; }
        public int PROPERTY_SCHEDULE_ID { get; set; }
        public decimal DEPRECIATION_AMOUNT { get; set; }
        public DateTime POSTING_DATE { get; set; }
        public int DEPRECIATION_YEAR { get; set; }
        public int DEPRECIATION_MONTH { get; set; }
    }
}
