using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYTRANSACTION
    {
        public int ID { get; set; }
        public string TRANSACTION_NUMBER { get; set; }
        public string TRAN_TYPE { get; set; } //Added 9/12/22 for Issuance Module
        public DateTime TRANSACTION_DATE { get; set; }
        public string FISCALPERIOD { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string MEMO { get; set; }
        public string RECEIVED_BY { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime DELIVERY_DATE { get; set; }
        public string USER_STAMP { get; set; }
        public string TIMESTAMP { get; set; }
        public string TRANSACTION_STATUS { get; set; }
        public string FUND_SOURCE { get; set; }
        public bool IS_PROPERTY { get; set; }
        public List <EAMISPROPERTYTRANSACTIONDETAILS> PROPERTY_TRANSACTION_DETAILS { get; set; }
        public List <EAMISSERVICELOGDETAILS> SERVICE_LOG_DETAILS { get; set; }


    }
}
