using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISSERIALTRAN
    {
        public int ID { get; set; }
        public int DELIVERY_RECEIPT_DETAILS_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public DateTime WARRANTY_EXPIRY_DATE { get; set; }
        public EAMISDELIVERYRECEIPTDETAILS DELIVERY_RECEIPT_DETAILS_GROUP { get; set; }

    }
}
