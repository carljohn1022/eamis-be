using System;
using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISDELIVERYRECEIPTDETAILS
    {
        public int ID { get; set; }
        public int DELIVERY_RECEIPT_ID { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public int QTY_ORDER { get; set; }
        public int QTY_DELIVERED { get; set; }
        public int QTY_REJECTED { get; set;}
        public int QTY_RECEIVED { get; set; }
        public decimal UNIT_COST { get; set; }
        public int SERIAL_LOT { get; set; }
        public string UNIT_OF_MEASUREMENT { get; set; }
        public decimal SUB_TOTAL { get; set; }
        public string USER_STAMP { get; set; }
        //public DateTime WARRANTY_EXPIRY_DATE { get; set; }
        public EAMISPROPERTYITEMS ITEMS_GROUP { get; set; }
        public EAMISDELIVERYRECEIPT DELIVERY_RECEIPT_GROUP { get;set; }
        public List<EAMISSERIALTRAN> SERIAL_TRAN { get; set; }
    }
}
