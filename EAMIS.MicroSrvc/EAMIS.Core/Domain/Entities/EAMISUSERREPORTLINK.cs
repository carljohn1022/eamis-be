using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISUSERREPORTLINK
    {
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public int REPORT_ID { get; set; }
        public bool CAN_VIEW { get; set; }
    }
}
