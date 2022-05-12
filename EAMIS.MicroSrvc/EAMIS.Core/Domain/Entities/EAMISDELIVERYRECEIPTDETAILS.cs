using System;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISDELIVERYRECEIPTDETAILS
    {
        public int ID { get; set; }
        public int DELIVERY_RECEIPT_ID { get; set; }
        public int ITEM_ID { get; set; }
        public int QTY_ORDER { get; set; }
        public int QTY_DELIVERED { get; set; }
        public int QTY_REJECTED { get; set;}
        public int QTY_RECEIVED { get; set; }
        public int UNIT_COST { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string UNIT_OF_MEASUREMENT { get; set; }
        public DateTime WARRANTY_EXPIRY_DATE { get; set; }
    }
}
