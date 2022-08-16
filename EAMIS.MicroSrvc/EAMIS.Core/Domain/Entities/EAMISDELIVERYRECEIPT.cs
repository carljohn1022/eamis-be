using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EAMIS.Core.Domain.Entities
{
    public class EAMISDELIVERYRECEIPT
    {
        public int ID { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string RECEIVED_BY { get; set; }
        [DataType(DataType.Date)]
        public DateTime DATE_RECEIVED { get; set; }
        public int SUPPLIER_ID { get; set; }
       
        public string PURCHASE_ORDER_NUMBER { get; set; }

        [DataType(DataType.Date)]
        public DateTime PURCHASE_ORDER_DATE { get; set; }

        public string PURCHASE_REQUEST_NUMBER { get; set; }

        [DataType(DataType.Date)]
        public DateTime PURCHASE_REQUEST_DATE { get; set; }

        public string SALE_INVOICE_NUMBER { get; set; }

        [DataType(DataType.Date)]
        public DateTime SALE_INVOICE_DATE { get; set; }

        public decimal TOTAL_AMOUNT { get; set; }
        public string  TRANSACTION_STATUS { get; set; }
        public int WAREHOUSE_ID { get; set; }

        public EAMISWAREHOUSE WAREHOUSE_GROUP { get; set; }
        public EAMISSUPPLIER SUPPLIER_GROUP { get; set; }
        public List<EAMISDELIVERYRECEIPTDETAILS> DELIVERY_RECEIPT_DETAILS { get; set; }

        

    }
}
