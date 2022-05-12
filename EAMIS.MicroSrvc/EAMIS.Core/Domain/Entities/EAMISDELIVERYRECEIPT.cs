using System;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISDELIVERYRECEIPT
    {
        public int ID { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string SUPPLIER { get; set; }
        public string PURCHASE_ORDER_NUMBER { get; set; }
        public DateTime PURCHASE_ORDER_DATE { get; set; }
        public string PURCHASE_REQUEST_NUMBER { get; set; }
        public DateTime PURCHASE_REQUEST_DATE { get; set; }
        public string SALE_INVOICE_NUMBER { get; set; }
        public DateTime SALE_INVOICE_DATE { get; set; }
        public int TOTAL_AMOUNT { get; set; }
        public string  TRANSACTION_STATUS { get; set; }
        public int SERIAL_LOT { get; set; }

    }
}
